using EasyDo.Module;
using System.Reflection;

namespace EasyDo.RedisCache
{
    [Depend(typeof(KernelModule))]
    public class RedisCahceModule: EasyDoModule
    {
        public override void Initialize()
        {
            iocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
