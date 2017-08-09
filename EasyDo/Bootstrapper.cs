using EasyDo.Dependency;
using EasyDo.Domain;
using EasyDo.Module;
using System;
using System.Reflection;

namespace EasyDo
{
    public class Bootstrapper : IDisposable
    {
        /// <summary>
        /// 入口模块
        /// </summary>
        internal Type StartupModule { get; }

        public IocManager IocManager { get; }

        private Bootstrapper(Type startupModule)
            : this(startupModule, IocManager.Instance)
        {

        }

        private Bootstrapper(Type startupModule, IocManager iocManager)
        {
            if (!typeof(EasyDoModule).GetTypeInfo().IsAssignableFrom(startupModule))
            {
                throw new ArgumentException($"{nameof(startupModule)}应该继承于 {nameof(EasyDoModule)}.");
            }

            StartupModule = startupModule;
            IocManager = iocManager;
        }
        public static Bootstrapper Create<TStartupModule>() where TStartupModule : EasyDoModule
        {
            return new Bootstrapper(typeof(TStartupModule));
        }

        public void Initialize()
        {
            //处理项目依赖模块
            var easyDoModuleManager = EasyDoModuleManager.Build(IocManager, StartupModule);

            //处理Entity 对象
            EntityManager.Bulid(IocManager, easyDoModuleManager.AllModuleAssembly);

            IocManager.InitContainer();
        }

        public void Dispose()
        {
            IocManager.Resolve<EasyDoModuleManager>().Shutdown();
            IocManager.Container.Dispose();
        }
    }
}
