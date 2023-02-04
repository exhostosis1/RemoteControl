using Moq;
using Servers.Middleware;
using Shared.DataObjects.Web;
using Shared.Logging.Interfaces;
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

    public LoggingMiddlewareTests()
    {
        _logger = Mock.Of<ILogger<LoggingMiddleware>>();
        _middleware = new LoggingMiddleware(_logger);
    }

    [Fact]
    public void RequestTest()
    {
        var response = new LocalResponse();

        _middleware.OnNext += (_, _) =>
        {
            response.ContentType = "test type";
            response.Payload = "test"u8.ToArray();
            response.StatusCode = HttpStatusCode.OK;
        };

        var context = new WebContext(new WebContextRequest("http://yo"), Mock.Of<WebContextResponse>());
        _middleware.ProcessRequest(null, context);

        Mock.Get(_logger).Verify(x => x.LogInfo(context.WebRequest.Path), Times.Once);
        Mock.Get(_logger).Verify(x => x.LogInfo($"{context.WebResponse.StatusCode}\n{context.WebResponse.ContentType}\n{context.WebResponse.Payload}"), Times.Once);
    }

    public void Dispose()
    {

    }
}