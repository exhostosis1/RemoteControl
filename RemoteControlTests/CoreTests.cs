using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteControlCore;

namespace RemoteControlTests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void StartRestartAndGetUriTest()
        {
            var core = new Core();

            core.Stop();

            try
            {
                core.Start();
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }

            var ub = core.GetUriBuilder();

            Assert.AreEqual(ub.Uri.ToString(), "http://localhost/");

            var scheme = "http";
            var host = "192.168.31.12";
            var port = 1488;
            
            try
            {
                core.Restart((scheme, host, port.ToString()));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            ub = core.GetUriBuilder();

            Assert.AreEqual(ub.Scheme, scheme);
            Assert.AreEqual(ub.Host, host);
            Assert.AreEqual(ub.Port, port);

            Assert.ThrowsException<FormatException>(() => core.Start(("http123", "localhost", "123fasdf")), "");
            Assert.ThrowsException<FormatException>(() => core.Restart(("http123", "localhost", "123fasdf")), "");
        }

        [TestMethod]
        public void StopListenTest()
        {
            var core = new Core();

            core.Start();

            try
            {
                core.Stop();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
