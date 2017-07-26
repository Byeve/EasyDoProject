using System;
using System.Collections.Generic;

namespace HY.Module
{
    public class HYModuleInfo
    {
        public bool Visited { get; set; }
        public List<Type> DependedModuleTypes { get; set; }
        public Type Type { get; set; }
        public HYModuleInfo()
        {
            DependedModuleTypes = new List<Type>();
        }
    }
}
