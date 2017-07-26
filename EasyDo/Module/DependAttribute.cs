using System;

namespace EasyDo.Module
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependAttribute: Attribute
    {
        public Type[] DependedModuleTypes { get; private set; }

        public DependAttribute(params Type[] dependedModuleTypes)
        {
            DependedModuleTypes = dependedModuleTypes;
        }
    }
}
