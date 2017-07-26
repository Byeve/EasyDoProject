using System;
using System.Collections.Generic;

namespace EasyDo.Module
{
    public class EasyDoModuleInfo
    {
        public bool Visited { get; set; }
        public List<Type> DependedModuleTypes { get; set; }
        public Type Type { get; set; }
        public EasyDoModuleInfo()
        {
            DependedModuleTypes = new List<Type>();
        }
    }
}
