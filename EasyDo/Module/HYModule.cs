using HY.Dependency;
using System.Reflection;

namespace HY.Module
{
    public abstract class HYModule
    {
        public IocManager iocManager { get; set; }
        public virtual void PreInitialize()
        {

        }
        public virtual void Initialize()
        {

        }
        public virtual void Shutdown()
        {

        }

        public virtual Assembly[] GetAssembly()
        {
            return new Assembly[0];
        }
    }
}
