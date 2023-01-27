using Moq;
using Servers.Middleware;
using Shared.DataObjects.Web;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;

namespace UnitTests.Middleware;

public class LoggingMiddlewareTests : IDisposable
{
    private class LocalResponse : WebContextResponse
    {
        public override void Close()
        {
        }
    }

    private readonly ILogger<LoggingMiddleware> _logger;
    private readonly LoggingMiddleware _middleware;
    private readonly Mock<IWebMiddleware> _next;

    public LoggingMiddlewareTests()
    {
        _logger = Mock.Of<ILogger<LoggingMiddleware>>();
        _next = new Mock<IWebMiddleware>(MockBehavior.Strict);
        _middleware = new LoggingMiddleware(_logger, _next.Object);
    }

    [Fact]
    public void RequestTest()
    {
        var response = new LocalResponse();
        
        _next.Setup(x => x.ProcessRequest(It.IsAny<WebContext>())).Callback(() =>
        {
            response.ContentType = "test type";
            response.Payload = "test"u8.ToArray();
            response.StatusCode = HttpStatusCode.OK;
        });

        var context = new WebContext(new WebContextRequest("http://yo"), Mock.Of<WebContextResponse>());
        _middleware.ProcessRequest(context);

        Mock.Get(_logger).Verify(x => x.LogInfo(context.WebRequest.Path), Times.Once);
        Mock.Get(_logger).Verify(x => x.LogInfo($"{context.WebResponse.StatusCode}\n{context.WebResponse.ContentType}\n{context.WebResponse.Payload}"), Times.Once);

        _next.Verify(x => x.ProcessRequest(context), Times.Once);
    }

    public void Dispose()
    {

    }
}