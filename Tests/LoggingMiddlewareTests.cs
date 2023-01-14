using Moq;
using Servers.Middleware;
using Shared.DataObjects.Http;
using Shared.Logging.Interfaces;

namespace Tests;

public class LoggingMiddlewareTests: IDisposable
{
    public LoggingMiddlewareTests()
    {

    }

    [Fact]
    public void RequestTest()
    {
        var logger = Mock.Of<ILogger<LoggingMiddleware>>();
        var middleware = new LoggingMiddleware(logger);

        var context = new Context("http://yo");
        middleware.ProcessRequest(context);
        var context1 = context;
        Mock.Get(logger).Verify(x => x.LogInfo(context1.Request.Path), Times.Exactly(1));
        Mock.Get(logger).Verify(x => x.LogInfo($"{context1.Response.StatusCode}\n{context1.Response.ContentType}\n{context1.Response.Payload}"), Times.Exactly(1));

        context = new Context("test contents");
        middleware.ProcessRequest(context);
        Mock.Get(logger).Verify(x => x.LogInfo(context.Request.Path), Times.Exactly(1));
        Mock.Get(logger).Verify(x => x.LogInfo($"{context.Response.StatusCode}\n{context.Response.ContentType}\n{context.Response.Payload}"), Times.Exactly(2));
    }

    public void Dispose()
    {

    }
}