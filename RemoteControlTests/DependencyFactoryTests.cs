using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyFactory;
using DependencyFactory.Config;

namespace RemoteControlTests
{
    interface TestInterface1
    { }

    interface TestInterface2
    { }

    class TestClass1 : TestInterface1
    {
        public TestClass1() { }

        public TestClass1(int one, string two) { }
    }

    class TestClass2 : TestInterface1
    {

    }

    class TestClass3 : TestInterface2
    {

    }

    [TestClass]
    public class DependencyFactoryTests
    {
        [TestMethod]
        public void NavigationPropertyTest()
        {
            var option1 = Factory.GetNavigationOption("opt1");

            var option3 = Factory.GetNavigationOption("opt1");

            Assert.AreSame(option1, option3);

            var option2 = Factory.GetNavigationOption("opt2");

            Assert.AreNotSame(option1, option2);
        }

        [TestMethod]
        public void FactoryConfigTest()
        {
            var option1 = Factory.GetNavigationOption("opt1");

            Factory.AddConfig<TestInterface1, TestClass1>();
            Assert.ThrowsException<Exception>(() => { Factory.AddConfig<TestInterface1, TestClass1>(); });

            try
            {
                Factory.AddConfig<TestInterface1, TestClass2>(option1, DependencyBehavior.Singleton);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.ThrowsException<ArgumentException>(() => { Factory.AddConfig<TestInterface2, TestClass2>(); });
        }

        [TestMethod]
        public void GetInstanceTest()
        {
            object inst;
            var option1 = Factory.GetNavigationOption("opt1");

            Assert.ThrowsException<ArgumentException>(() => { inst = Factory.GetInstance<object>(); });
                        
            inst = Factory.GetInstance<TestInterface1>(1, "b");

            Assert.IsInstanceOfType(inst, typeof(TestClass1));

            var inst2 = Factory.GetInstance<TestInterface1>(1, "b");

            Assert.AreNotSame(inst, inst2);

            inst = Factory.GetInstance<TestInterface1>(option1);
            inst2 = Factory.GetInstance<TestInterface1>(option1);

            Assert.AreSame(inst, inst2);
        }
    }
}
