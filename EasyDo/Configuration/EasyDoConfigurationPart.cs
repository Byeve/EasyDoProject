using System;

namespace EasyDo.Configuration
{
    public partial class EasyDoConfigurationPart
    {
        internal EasyDoConfiguration Configuration { get; }
        internal EasyDoConfigurationPart()
        {
            Configuration = new EasyDoConfiguration();
        }

        /// <summary>
        /// 配置配置数据库信息
        /// </summary>
        /// <param name="dataBaseName">数据库名称</param>
        /// <param name="dataBaseConnection">数据库连接字符串</param>
        /// <returns></returns>
        public EasyDoConfigurationPart DataBaseConfigurations(string dataBaseName, string primarydataBaseConnection, string secondaryDataBaseConnection = "")
        {
            var dataBaseConfiguration = Configuration.DataBaseConfigurations.Find(m => m.DataBaseName == dataBaseName);
            if (dataBaseConfiguration!=null)
            {
                Configuration.DataBaseConfigurations.Remove(dataBaseConfiguration);
            }

            Configuration.DataBaseConfigurations.Add(new DataBaseConfiguration { DataBaseName = dataBaseName, PrimaryDataBaseConnection = primarydataBaseConnection, SecondaryDataBaseConnection  = secondaryDataBaseConnection });
            return this;
        }

        /// <summary>
        /// 启用从库
        /// </summary>
        /// <param name="dataBaseName">数据库名称</param>
        /// <returns></returns>
        public EasyDoConfigurationPart UseSecondaryDB(string dataBaseName)
        {
            var dataBaseConfiguration = Configuration.DataBaseConfigurations.Find(m => m.DataBaseName == dataBaseName);
            if (dataBaseConfiguration == null || string.IsNullOrEmpty( dataBaseConfiguration.SecondaryDataBaseConnection))
            {
                throw new ArgumentException(string.Format("数据库：{0} 从库连接未配置！", dataBaseName));
            }
            Configuration.EnableSecondaryDB = true;
            return this;
        }

        /// <summary>
        /// 配置redis信息
        /// </summary>
        /// <param name="host">host</param>
        /// <param name="port">port</param>
        /// <param name="dataBaseIndex">dataBaseIndex</param>
        /// <returns></returns>
        public EasyDoConfigurationPart RedisConfigurations(string host, int port, int dataBaseIndex=0)
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
