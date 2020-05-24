using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteControlCore.Listeners;

namespace RemoteControlCore.Tests
{
    [TestClass]
    public class MyHttpListenerTests
    {
        readonly MyHttpListener _listener = new MyHttpListener();

        [TestMethod]
        public void StartListenTest()
        {
            try
            {
                _listener.StopListen();
                _listener.StartListen();
            }
            catch
            {
                Assert.Fail();
            }

            _listener.StopListen();
        }

        [TestMethod]
        public void RetartListenTest()
        {
            try
            {
                _listener.RestartListen();
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
                _listener.StopListen();
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}
