using System.Collections.Generic;
using System.Linq;
namespace EasyDo.Configuration
{
    public class EasyDoConfiguration
    {
        //配置数据库连接
        internal List<DataBaseConfiguration> DataBaseConfigurations { get; set; }
        /// <summary>
        /// redids 配置信息
        /// </summary>
        internal List<RedisConfiguration> RedisConfigurations { get; set; }

        internal EasyDoConfiguration()
        {
            DataBaseConfigurations = new List<DataBaseConfiguration>();
            RedisConfigurations = new List<RedisConfiguration>();
        }
        /// <summary>
        /// 是否使用Redis
        /// </summary>
        public bool EnableRedis { get; internal set; }
        /// <summary>
        /// 根据数据库名称 获取数据库连接
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        /// <returns>数据库连接字符串</returns>
        public string DatabaseConnectionString(string databaseName)
        {
            var dataBaseConfiguration = DataBaseConfigurations.Find(m => m.DataBaseName == databaseName);
            if (dataBaseConfiguration == null)
            {
                return "";
            }
            return dataBaseConfiguration.DataBaseConnection;
        }
        /// <summary>
        /// redis 配置信息
        /// </summary>
        /// <returns></returns>
        public string[] RedisConnectionStrings()
        {
            return RedisConfigurations.Select(m => m.Host + ":" + m.Port).ToArray();
        }
    }
    /// <summary>
    /// 数据库配置信息
    /// </summary>
    internal class DataBaseConfiguration
    {
        public string DataBaseName { get; set; }
        public string DataBaseConnection { get; set; }
    }
    /// <summary>
    /// redis配置信息
    /// </summary>
    internal class RedisConfiguration
    {
        public int Port { get; set; }
        public int DataBaseIndex { get; set; }
        public string Host { get; set; }
    }
}
