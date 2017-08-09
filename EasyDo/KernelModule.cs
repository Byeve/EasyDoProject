using Autofac;
using EasyDo.Logging;
using EasyDo.Module;
using System.Reflection;

namespace EasyDo
{
    public class KernelModule : EasyDoModule
    {
        public override void PreInitialize()
        {
          IocManager.ContainerBuilder.RegisterModule(new Log4NetModule());
        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
