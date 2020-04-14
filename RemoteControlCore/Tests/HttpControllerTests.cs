using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteControlCore.Controllers;
using Moq;
using RemoteControlCore.Interfaces;
using System.Net;

namespace RemoteControlCore.Tests
{
    [TestClass]
    public class HttpControllerTests
    {
        HttpController controller = new HttpController();

        [TestMethod]
        public void ProcessRequestTest()
        {
            var context = new Mock<IHttpRequestArgs>();

            context.SetupGet(x => new Mock<HttpListenerRequest>().Object);
            context.SetupGet(x => new Mock<HttpListenerResponse>().Object);

            try
            {
                controller.ProcessRequest(context.Object);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
