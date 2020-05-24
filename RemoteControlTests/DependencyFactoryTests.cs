using DependencyFactory;
using DependencyFactory.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RemoteControlTests
{
    internal interface ITestInterface1
    { }

    internal interface ITestInterface2
    { }

    internal class TestClass1 : ITestInterface1
    {
        public readonly int A;
        public readonly string B;
        public TestClass1() { }

        public TestClass1(int a, string b)
        {
            A = a;
            B = b;
        }
    }

    internal abstract class TestClass2 : ITestInterface1
    {

    }

    internal class TestClass3 : ITestInterface2
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

            Factory.AddConfig<ITestInterface1, TestClass1>();
            Assert.ThrowsException<Exception>(() => { Factory.AddConfig<ITestInterface1, TestClass1>(); });

            try
            {
                Factory.AddConfig<ITestInterface1, TestClass2>(option1, DependencyBehavior.Singleton);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            Assert.ThrowsException<ArgumentException>(() => { Factory.AddConfig<ITestInterface2, TestClass2>(); });
        }

        [TestMethod]
        public void GetInstanceTest()
        {
            object inst;
            var option1 = Factory.GetNavigationOption("opt1");

            Assert.ThrowsException<ArgumentException>(() => { inst = Factory.GetInstance<object>(); });
                        
            inst = Factory.GetInstance<ITestInterface1>(1, "b");

            Assert.IsInstanceOfType(inst, typeof(TestClass1));

            var inst2 = Factory.GetInstance<ITestInterface1>(1, "b");

            Assert.AreNotSame(inst, inst2);
            Assert.AreEqual(((TestClass1)inst2).A, 1);
            Assert.AreEqual(((TestClass1)inst2).B, "b");

            inst = Factory.GetInstance<ITestInterface1>(option1);
            inst2 = Factory.GetInstance<ITestInterface1>(option1);

            Assert.AreSame(inst, inst2);
        }
    }
}
