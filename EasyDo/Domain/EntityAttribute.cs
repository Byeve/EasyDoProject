using System;

namespace EasyDo.Domain
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityAttribute : Attribute
    {
        public string DbName { get; set; }
        public string TableName { get; set; }
    }
}
