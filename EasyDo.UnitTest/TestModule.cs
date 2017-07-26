using EasyDo.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyDo.UnitTest
{
    [Depend(typeof(KernelModule))]
    public class TestModule:EasyDo.Module.EasyDoModule
    {
        public override void PreInitialize()
        {
            iocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
