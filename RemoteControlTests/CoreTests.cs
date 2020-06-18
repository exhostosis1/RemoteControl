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
            var core = new Core(false);

            core.Stop();

            try
            {
                core.Start();
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }

            var ub = core.GetCurrentConfig().Item1;

            Assert.AreEqual(ub.Uri.ToString(), "http://localhost/");

            var scheme = "http";
            var host = "192.168.31.12";
            var port = 1488;
            
            try
            {
                core.Restart(new UriBuilder(scheme, host, port), false);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            ub = core.GetCurrentConfig().Item1;

            Assert.AreEqual(ub.Scheme, scheme);
            Assert.AreEqual(ub.Host, host);
            Assert.AreEqual(ub.Port, port);

            Assert.ThrowsException<FormatException>(() => core.Start(new UriBuilder("http123", "localhost", 123), false), "");
            Assert.ThrowsException<FormatException>(() => core.Restart(new UriBuilder("http123", "localhost", 123), false), "");
        }

        [TestMethod]
        public void StopListenTest()
        {
            var core = new Core(false);

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
