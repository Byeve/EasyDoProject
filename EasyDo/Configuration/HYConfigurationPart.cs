﻿namespace HY.Configuration
{
    public partial class HYConfigurationPart
    {
        internal HYConfiguration Configuration { get; }
        internal HYConfigurationPart()
        {
            Configuration = new HYConfiguration();
        }
        /// <summary>
        /// 配置配置数据库信息
        /// </summary>
        /// <param name="dataBaseName">数据库名称</param>
        /// <param name="dataBaseConnection">数据库连接字符串</param>
        /// <returns></returns>
        public HYConfigurationPart DataBaseConfigurations(string dataBaseName, string dataBaseConnection)
        {
            var dataBaseConfiguration = Configuration.DataBaseConfigurations.Find(m => m.DataBaseName == dataBaseName);
            if (dataBaseConfiguration!=null)
            {
                Configuration.DataBaseConfigurations.Remove(dataBaseConfiguration);
            }

            Configuration.DataBaseConfigurations.Add(new DataBaseConfiguration { DataBaseName = dataBaseName, DataBaseConnection = dataBaseConnection });
            return this;
        }
        /// <summary>
        /// 配置redis信息
        /// </summary>
        /// <param name="host">host</param>
        /// <param name="port">port</param>
        /// <param name="dataBaseIndex">dataBaseIndex</param>
        /// <returns></returns>
        public HYConfigurationPart RedisConfigurations(string host, int port, int dataBaseIndex=0)
        {
            Configuration.EnableRedis = true;
            var redisConfigurations = Configuration.RedisConfigurations.Find(m => m.Host == host);
            if (redisConfigurations != null)
            {
                Configuration.RedisConfigurations.Remove(redisConfigurations);
            }

            Configuration.RedisConfigurations.Add(new RedisConfiguration { Host = host,  Port = port, DataBaseIndex = dataBaseIndex });
            return this;
        }
    }
}
