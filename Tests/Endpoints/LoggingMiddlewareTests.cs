using Moq;
using Servers.Middleware;
using Shared.DataObjects.Http;
using Shared.Logging.Interfaces;

namespace Tests.Endpoints;

public class LoggingMiddlewareTests : IDisposable
{
    public LoggingMiddlewareTests()
    {

    }

    [Fact]
    public void RequestTest()
    {
        var logger = Mock.Of<ILogger<LoggingMiddleware>>();
        var middleware = new LoggingMiddleware(logger);

        var response = Mock.Of<HttpContextResponse>();

        var context = new HttpContext(new HttpContextRequest("http://yo"), response);
        middleware.ProcessRequest(context);
        var context1 = context;
        Mock.Get(logger).Verify(x => x.LogInfo(context1.HttpRequest.Path), Times.Exactly(1));
        Mock.Get(logger).Verify(x => x.LogInfo($"{context1.HttpResponse.StatusCode}\n{context1.HttpResponse.ContentType}\n{context1.HttpResponse.Payload}"), Times.Exactly(1));

        context = new HttpContext(new HttpContextRequest("test contents"), response);
        middleware.ProcessRequest(context);
        Mock.Get(logger).Verify(x => x.LogInfo(context.HttpRequest.Path), Times.Exactly(1));
        Mock.Get(logger).Verify(x => x.LogInfo($"{context.HttpResponse.StatusCode}\n{context.HttpResponse.ContentType}\n{context.HttpResponse.Payload}"), Times.Exactly(2));
    }

    public void Dispose()
    {

    }
}