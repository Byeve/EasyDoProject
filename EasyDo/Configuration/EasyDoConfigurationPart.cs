using System;
using System.Collections.Generic;
using System.Linq;

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
        /// 配置主数据库信息（可重复调用配置多个数据库）
        /// </summary>
        /// <param name="dataBaseName">数据库名称</param>
        /// <param name="dataBaseConnection">数据库连接字符串</param>
        /// <returns></returns>
        public EasyDoConfigurationPart MasterDataBases(string dataBaseName, string dbConnectionString)
        {
            if (string.IsNullOrEmpty(dataBaseName))
            {
                throw new ArgumentException(string.Format("数据库名称不能为空！", dataBaseName));
            }

            if (string.IsNullOrEmpty(dbConnectionString))
            {
                throw new ArgumentException(string.Format("数据库：{0} 主库连接不能为空！", dataBaseName));
            }

            var dataBaseConfiguration = Configuration.DataBaseConfigurations.Find(m => m.DataBaseName == dataBaseName);
            if (dataBaseConfiguration!=null)
            {
                Configuration.DataBaseConfigurations.Remove(dataBaseConfiguration);
            }

            dataBaseConfiguration = new DataBaseConfiguration
            {
                DataBaseName = dataBaseName,
                MasterDataBaseConnectionString = dbConnectionString,
                SlaveDataBaseConnectionStrings = new List<string>()
            };

            Configuration.DataBaseConfigurations.Add(dataBaseConfiguration);
            return this;
        }

        /// <summary>
        /// 配置从数据库信息（可重复调用配置多个数据库）
        /// </summary>
        /// <param name="dataBaseName">数据库名称</param>
        /// <param name="dataBaseConnection">数据库连接字符串</param>
        /// <returns></returns>
        public EasyDoConfigurationPart SlaveDataBaseBases(string dataBaseName, string dbConnectionString)
        {
            if (string.IsNullOrEmpty(dataBaseName))
            {
                throw new ArgumentException(string.Format("数据库名称不能为空！", dataBaseName));
            }

            if (string.IsNullOrEmpty(dbConnectionString))
            {
                throw new ArgumentException(string.Format("数据库：{0} 数据库连接不能为空！", dataBaseName));
            }

            var dataBaseConfiguration = Configuration.DataBaseConfigurations.Find(m => m.DataBaseName == dataBaseName);
            if (dataBaseConfiguration == null)
            {
                throw new ArgumentException(string.Format("数据库：{0} 未配置！查找不到数据库信息", dataBaseName));
            }

            if (!dataBaseConfiguration.SlaveDataBaseConnectionStrings.Contains(dbConnectionString))
            {
                dataBaseConfiguration.SlaveDataBaseConnectionStrings.Add(dbConnectionString);
            }
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
            if (dataBaseConfiguration == null)
            {
                throw new ArgumentException(string.Format("数据库：{0} 未配置！查找不到数据库信息", dataBaseName));
            }
            if (!dataBaseConfiguration.SlaveDataBaseConnectionStrings.Any())
            {
                throw new ArgumentException(string.Format("数据库：{0} 必须配置至少一个从库连接字符串！", dataBaseName));
            }
            dataBaseConfiguration.EnableSecondaryDB = true;
            return this;
        }

        /// <summary>
        /// 禁止软删除
        /// </summary>
        /// <param name="dataBaseName">数据库名称</param>
        /// <returns></returns>
        public EasyDoConfigurationPart DisableSoftDelete(string dataBaseName)
        {
            var dataBaseConfiguration = Configuration.DataBaseConfigurations.Find(m => m.DataBaseName == dataBaseName);
            if (dataBaseConfiguration == null)
            {
                throw new ArgumentException(string.Format("数据库：{0} 未配置！查找不到数据库信息", dataBaseName));
            }
            dataBaseConfiguration.DisableSoftDelete = true;
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
