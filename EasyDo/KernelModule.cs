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
          iocManager.ContainerBuilder.RegisterModule(new Log4netModule());
        }
        public override void Initialize()
        {
            iocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
