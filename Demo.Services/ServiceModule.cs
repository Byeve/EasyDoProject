using Demo.Models;
using EasyDo.Module;
using EasyDo.Mongo;
using System.Reflection;

namespace Demo.Services
{
    [Depend(typeof(MongoModule), typeof(ModelsModule))]
    public class ServiceModule:EasyDo.Module.EasyDoModule
    {
        public override void Initialize()
        {
            iocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
