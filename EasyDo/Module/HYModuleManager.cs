using Autofac;
using HY.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HY.Module
{
    public class HYModuleManager
    {
        private List<HYModule> modules;

        internal List<Assembly> AllModuleAssembly { get; private set; }

        private HYModuleManager()
        {
            AllModuleAssembly = new List<Assembly>();
        }

        /// <summary>
        /// 初始化依赖模块
        /// </summary>
        /// <param name="iocManager">ioc容器</param>
        /// <param name="startupModule">启动模块</param>
        public static HYModuleManager Build(IocManager iocManager, Type startupModule)
        {
            var pandaModuleManager = new HYModuleManager();

            if (!pandaModuleManager.IsHcModule(startupModule))
            {
                throw new ArgumentException(string.Format("模块:{0}不能是HcModule类型", startupModule.Name));
            }
            var hcModuleInfos = pandaModuleManager.FindDependedModuleTypesRecursivelyIncludingGivenModule(startupModule);

            pandaModuleManager.modules = new List<HYModule>();
            foreach (var moduleInfo in hcModuleInfos)
            {
                var module = Activator.CreateInstance(moduleInfo.Type) as HYModule;

                pandaModuleManager.AllModuleAssembly.Add(moduleInfo.Type.Assembly);

                module.iocManager = iocManager;
                pandaModuleManager.modules.Add(module);
            }

            pandaModuleManager.modules.ForEach(m => m.PreInitialize());
            pandaModuleManager.modules.ForEach(m => m.Initialize());

            //自我注册
            iocManager.ContainerBuilder.RegisterInstance(pandaModuleManager).SingleInstance();

            return pandaModuleManager;
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
                typeof(HYModule).IsAssignableFrom(type);
        }

        private List<HYModuleInfo> FindDependedModuleTypesRecursivelyIncludingGivenModule(Type moduleType)
        {
            var hcModuleInfos = new List<HYModuleInfo>();
            AddModuleAndDependenciesRecursively(hcModuleInfos, moduleType);

            var SortHcModuleInfos = new List<HYModuleInfo>();

            SortByDependencies(hcModuleInfos.Find(m => m.Type == moduleType), hcModuleInfos, SortHcModuleInfos);

            return SortHcModuleInfos;
        }

        private void AddModuleAndDependenciesRecursively(List<HYModuleInfo> HcModuleInfos, Type module)
        {
            if (!IsHcModule(module))
            {
                throw new ArgumentException("This type is not an HY module: " + module.AssemblyQualifiedName);
            }

            if (HcModuleInfos.Find(m => m.Type == module) != null)
            {
                return;
            }

            var dependedModules = FindDependedModuleTypes(module);

            HcModuleInfos.Add(new HYModuleInfo { DependedModuleTypes = dependedModules, Type = module });

            foreach (var dependedModule in dependedModules)
            {
                AddModuleAndDependenciesRecursively(HcModuleInfos, dependedModule);
            }
        }

        private List<Type> FindDependedModuleTypes(Type moduleType)
        {
            if (!IsHcModule(moduleType))
            {
                throw new ArgumentException("This type is not an HY module: " + moduleType.AssemblyQualifiedName);
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
        private void SortByDependencies(HYModuleInfo hcModuleInfo, List<HYModuleInfo> NoSortHcModuleInfos, List<HYModuleInfo> SortHcModuleInfos)
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
