using EasyDo.Dependency;
using System.Reflection;

namespace EasyDo.Module
{
    public abstract class EasyDoModule
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
