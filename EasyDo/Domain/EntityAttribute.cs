using System;

namespace EasyDo.Domain
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityAttribute : Attribute
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 启用主从 并且属性值为true读从库
        /// </summary>
        public bool ReadSecondary { get; set; }
    }
}
