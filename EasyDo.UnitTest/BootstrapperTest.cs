using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDo.UnitTest
{
    [TestClass]
    public class BootstrapperTest
    {
        public static Bootstrapper _bootstrapper { get; } = Bootstrapper.Create<TestModule>();
        [TestInitialize]
        public void Init()
        {
            _bootstrapper.Initialize();
        }

        [TestMethod]
        public void TestMethod1()
        {
           
        }
        [TestCleanup]
        public void ShutDownd()
        {
            _bootstrapper.Dispose();
        }
    }
}
