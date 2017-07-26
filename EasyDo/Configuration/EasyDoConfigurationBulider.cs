using Autofac;
using EasyDo.Dependency;
using System;
namespace EasyDo.Configuration
{
    /// <summary>
    /// EasyDo  数据库配置  缓存配置等
    /// </summary>
    public static class EasyDoConfigurationBulider
    {
        public static void Bulid(Action<EasyDoConfigurationPart> settings)
        {
            var pcp = new EasyDoConfigurationPart();
            settings(pcp);
            //自我注册
            IocManager.Instance.ContainerBuilder.RegisterInstance(pcp.Configuration).SingleInstance();
        }
    }


}
