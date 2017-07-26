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
        private List<EasyDoModule> modules;

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
            var EasyDoModuleManager = new EasyDoModuleManager();

            if (!EasyDoModuleManager.IsHcModule(startupModule))
            {
                throw new ArgumentException(string.Format("模块:{0}不能是HcModule类型", startupModule.Name));
            }
            var hcModuleInfos = EasyDoModuleManager.FindDependedModuleTypesRecursivelyIncludingGivenModule(startupModule);

            EasyDoModuleManager.modules = new List<EasyDoModule>();
            foreach (var moduleInfo in hcModuleInfos)
            {
                var module = Activator.CreateInstance(moduleInfo.Type) as EasyDoModule;

                EasyDoModuleManager.AllModuleAssembly.Add(moduleInfo.Type.Assembly);

                module.iocManager = iocManager;
                EasyDoModuleManager.modules.Add(module);
            }

            EasyDoModuleManager.modules.ForEach(m => m.PreInitialize());
            EasyDoModuleManager.modules.ForEach(m => m.Initialize());

            //自我注册
            iocManager.ContainerBuilder.RegisterInstance(EasyDoModuleManager).SingleInstance();

            return EasyDoModuleManager;
        }

        /// <summary>
        /// 释放所有模块
        /// </summary>
        public void Shutdown()
        {
            modules.ForEach(m => m.Shutdown());
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

            var SortHcModuleInfos = new List<EasyDoModuleInfo>();

            SortByDependencies(hcModuleInfos.Find(m => m.Type == moduleType), hcModuleInfos, SortHcModuleInfos);

            return SortHcModuleInfos;
        }

        private void AddModuleAndDependenciesRecursively(List<EasyDoModuleInfo> HcModuleInfos, Type module)
        {
            if (!IsHcModule(module))
            {
                throw new ArgumentException("This type is not an EasyDo module: " + module.AssemblyQualifiedName);
            }

            if (HcModuleInfos.Find(m => m.Type == module) != null)
            {
                return;
            }

            var dependedModules = FindDependedModuleTypes(module);

            HcModuleInfos.Add(new EasyDoModuleInfo { DependedModuleTypes = dependedModules, Type = module });

            foreach (var dependedModule in dependedModules)
            {
                AddModuleAndDependenciesRecursively(HcModuleInfos, dependedModule);
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
        /// <param name="HcModuleInfos">模块集合</param>
        /// <returns>排序模块集合</returns>
        private void SortByDependencies(EasyDoModuleInfo hcModuleInfo, List<EasyDoModuleInfo> NoSortHcModuleInfos, List<EasyDoModuleInfo> SortHcModuleInfos)
        {
            if (hcModuleInfo.Visited)
            {
                return;
            }

            hcModuleInfo.Visited = true;
            if (!SortHcModuleInfos.Contains(hcModuleInfo))
            {
                SortHcModuleInfos.Add(hcModuleInfo);
            }


            foreach (var type in hcModuleInfo.DependedModuleTypes)
            {
                var find = SortHcModuleInfos.Find(m => m.Type == type);
                if (find != null)
                {
                    SortHcModuleInfos.Remove(find);
                }
                var index = SortHcModuleInfos.FindIndex(m => m.Type == hcModuleInfo.Type);
                SortHcModuleInfos.Insert(index, NoSortHcModuleInfos.Find(m => m.Type == type));
            }

            if (SortHcModuleInfos.Find(m => m.Visited == false) == null)
            {
                return;
            }
            var moduleInfo = SortHcModuleInfos.FindLast(m => m.Visited == false);
            SortByDependencies(moduleInfo, NoSortHcModuleInfos, SortHcModuleInfos);
        }


    }
}
