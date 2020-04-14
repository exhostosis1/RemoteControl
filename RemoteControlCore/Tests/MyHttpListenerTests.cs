using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteControlCore.Listeners;
using RemoteControlCore.Interfaces;

namespace RemoteControlCore.Tests
{
    [TestClass]
    public class MyHttpListenerTests
    {
        MyHttpListener listener = new MyHttpListener();

        [TestMethod]
        public void StartListenTest()
        {
            try
            {
                listener.StopListen();
                listener.StartListen();
            }
            catch
            {
                Assert.Fail();
            }

            listener.StopListen();
        }

        [TestMethod]
        public void RetartListenTest()
        {
            try
            {
                listener.RestartListen();
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void StopListenTest()
        {
            try
            {
                listener.StopListen();
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}
