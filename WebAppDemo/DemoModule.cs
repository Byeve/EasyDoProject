using Demo.Services;
using EasyDo.Configuration;
using EasyDo.Module;
using EasyDo.Mvc;
using EasyDo.RedisCache;
using System.Configuration;
using System.Reflection;

namespace WebAppDemo
{
    [Depend(typeof(MvcModule), typeof(ServiceModule), typeof(RedisCahceModule))]
    public class DemoModule : EasyDoModule
    {
        public override void PreInitialize()
        {
            var dbcon = ConfigurationManager.AppSettings["HC"] ?? "";
            var RedisCacheHost = ConfigurationManager.AppSettings["RedisCacheHost"] ?? "";
            EasyDoConfigurationBulider.Bulid(settings =>
            {
                settings.DataBaseConfigurations("HC", dbcon,"")
                .RedisConfigurations(RedisCacheHost, 6379);
            });
        }
        public override void Initialize()
        {
            iocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

        }
    }
}