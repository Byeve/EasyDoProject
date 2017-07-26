using Autofac;
using HY.Dependency;
using System;
namespace HY.Configuration
{
    /// <summary>
    /// HY  数据库配置  缓存配置等
    /// </summary>
    public static class HYConfigurationBulider
    {
        public static void Bulid(Action<HYConfigurationPart> settings)
        {
            var pcp = new HYConfigurationPart();
            settings(pcp);
            //自我注册
            IocManager.Instance.ContainerBuilder.RegisterInstance(pcp.Configuration).SingleInstance();
        }
    }


}
