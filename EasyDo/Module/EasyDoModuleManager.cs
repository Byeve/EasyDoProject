using Autofac;
using EasyDo.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyDo.Module
{
    public class EasyDoModuleManager
    {
        private List<EasyDoModule> _modules;

        internal List<Assembly> AllModuleAssembly { get; private set; }

        private EasyDoModuleManager()
        {
            AllModuleAssembly = new List<Assembly>();
        }

        /// <summary>
        /// 初始化依赖模块
        /// </summary>
        /// <param name="iocManager">ioc容器</param>
        /// <param name="startupModule">启动模块</param>
        public static EasyDoModuleManager Build(IocManager iocManager, Type startupModule)
        {
            var easyDoModuleManager = new EasyDoModuleManager();

            if (!easyDoModuleManager.IsHcModule(startupModule))
            {
                throw new ArgumentException($"模块:{startupModule.Name}不能是HcModule类型");
            }
            var hcModuleInfos = easyDoModuleManager.FindDependedModuleTypesRecursivelyIncludingGivenModule(startupModule);

            easyDoModuleManager._modules = new List<EasyDoModule>();
            foreach (var moduleInfo in hcModuleInfos)
            {
                var module = Activator.CreateInstance(moduleInfo.Type) as EasyDoModule;

                easyDoModuleManager.AllModuleAssembly.Add(moduleInfo.Type.Assembly);

                if (module != null)
                {
                    module.IocManager = iocManager;
                    easyDoModuleManager._modules.Add(module);
                }
            }

            easyDoModuleManager._modules.ForEach(m => m.PreInitialize());
            easyDoModuleManager._modules.ForEach(m => m.Initialize());

            //自我注册
            iocManager.ContainerBuilder.RegisterInstance(easyDoModuleManager).SingleInstance();

            return easyDoModuleManager;
        }

        /// <summary>
        /// 释放所有模块
        /// </summary>
        public void Shutdown()
        {
            _modules.ForEach(m => m.Shutdown());
        }

        private bool IsHcModule(Type type)
        {
            return
                type.IsClass &&
                !type.IsAbstract &&
                !type.IsGenericType &&
                typeof(EasyDoModule).IsAssignableFrom(type);
        }

        private List<EasyDoModuleInfo> FindDependedModuleTypesRecursivelyIncludingGivenModule(Type moduleType)
        {
            var hcModuleInfos = new List<EasyDoModuleInfo>();
            AddModuleAndDependenciesRecursively(hcModuleInfos, moduleType);

            var sortHcModuleInfos = new List<EasyDoModuleInfo>();

            SortByDependencies(hcModuleInfos.Find(m => m.Type == moduleType), hcModuleInfos, sortHcModuleInfos);

            return sortHcModuleInfos;
        }

        private void AddModuleAndDependenciesRecursively(List<EasyDoModuleInfo> hcModuleInfos, Type module)
        {
            if (!IsHcModule(module))
            {
                throw new ArgumentException("This type is not an EasyDo module: " + module.AssemblyQualifiedName);
            }

            if (hcModuleInfos.Find(m => m.Type == module) != null)
            {
                return;
            }

            var dependedModules = FindDependedModuleTypes(module);

            hcModuleInfos.Add(new EasyDoModuleInfo { DependedModuleTypes = dependedModules, Type = module });

            foreach (var dependedModule in dependedModules)
            {
                AddModuleAndDependenciesRecursively(hcModuleInfos, dependedModule);
            }
        }

        private List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsHcModule(moduleType))
            {
                throw new ArgumentException("This type is not an EasyDo module: " + moduleType.AssemblyQualifiedName);
            }

            var list = new List<Type>();

            if (moduleType.GetTypeInfo().IsDefined(typeof(DependAttribute), true))
            {
                var dependsOnAttributes = moduleType.GetTypeInfo().GetCustomAttributes(typeof(DependAttribute), true).Cast<DependAttribute>();
                foreach (var dependsOnAttribute in dependsOnAttributes)
                {
                    foreach (var dependedModuleType in dependsOnAttribute.DependedModuleTypes)
                    {
                        list.Add(dependedModuleType);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 依赖模块进行排序
        /// </summary>
        /// <param name="hcModuleInfo">模块集合</param>
        /// <param name="noSortHcModuleInfos"></param>
        /// <param name="sortHcModuleInfos"></param>
        /// <returns>排序模块集合</returns>
        private void SortByDependencies(EasyDoModuleInfo hcModuleInfo, List<EasyDoModuleInfo> noSortHcModuleInfos, List<EasyDoModuleInfo> sortHcModuleInfos)
        {
            if (hcModuleInfo.Visited)
            {
                return;
            }

            hcModuleInfo.Visited = true;
            if (!sortHcModuleInfos.Contains(hcModuleInfo))
            {
                sortHcModuleInfos.Add(hcModuleInfo);
            }


            foreach (var type in hcModuleInfo.DependedModuleTypes)
            {
                var find = sortHcModuleInfos.Find(m => m.Type == type);
                if (find != null)
                {
                    sortHcModuleInfos.Remove(find);
                }
                var index = sortHcModuleInfos.FindIndex(m => m.Type == hcModuleInfo.Type);
                sortHcModuleInfos.Insert(index, noSortHcModuleInfos.Find(m => m.Type == type));
            }

            if (sortHcModuleInfos.Find(m => m.Visited == false) == null)
            {
                return;
            }
            var moduleInfo = sortHcModuleInfos.FindLast(m => m.Visited == false);
            SortByDependencies(moduleInfo, noSortHcModuleInfos, sortHcModuleInfos);
        }


    }
}
