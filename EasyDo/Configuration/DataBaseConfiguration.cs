using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDo.Configuration
{
    /// <summary>
    /// 数据库配置信息
    /// </summary>
    internal class DataBaseConfiguration
    {
        public string DataBaseName { get; set; }

        public string MasterDataBaseConnectionString { get; set; }

        public List<string> SlaveDataBaseConnectionStrings { get; set; }

        public bool EnableSecondaryDB { get; set; }

        public bool DisableSoftDelete { get; set; }
    }
}
