using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDo.Configuration
{

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
