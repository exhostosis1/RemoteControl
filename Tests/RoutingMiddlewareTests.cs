using Moq;
using Servers.Middleware;
using Shared.DataObjects.Http;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Tests;

public class RoutingMiddlewareTests: IDisposable
{
    public RoutingMiddlewareTests()
    {

    }

    private class MockApiV1Endpoint: IApiEndpoint
    {
        public string ApiVersion => "v1";
        public int Count;

        public void ProcessRequest(Context context)
        {
            Count++;
        }
    }

    private class MockApiV2Endpoint: IApiEndpoint
    {
        public string ApiVersion => "v2";
        public int Count;

        public void ProcessRequest(Context context)
        {
            Count++;
        }
    }

    private class MockStaticEndpoint: IEndpoint
    {
        public int Count;

        public void ProcessRequest(Context context)
        {
            Count++;
        }
    }

    [Fact]
    public void RequestTest()
    {
        var logger = Mock.Of<ILogger<RoutingMiddleware>>();
        var v1Endpoint = new MockApiV1Endpoint();
        var v2Endpoint = new MockApiV2Endpoint();
        var staticEndpoint = new MockStaticEndpoint();
        
        var endpoints = new List<IEndpoint>
        {
            v1Endpoint,
            v2Endpoint,
            staticEndpoint
        };

        var middleware = new RoutingMiddleware(endpoints, logger);

        var context = new Context("http://localhost/api/v2/controller/method");
        middleware.ProcessRequest(context);

        Assert.True(v1Endpoint.Count == 0);
        Assert.True(v2Endpoint.Count == 1);
        Assert.True(staticEndpoint.Count == 0);

        context = new Context("asdff/st/api/v1/controller/method/param");
        middleware.ProcessRequest(context);

        Assert.True(v1Endpoint.Count == 1);
        Assert.True(v2Endpoint.Count == 1);
        Assert.True(staticEndpoint.Count == 0);

        context = new Context("http://localhost/controller/method");
        middleware.ProcessRequest(context);

        Assert.True(v1Endpoint.Count == 1);
        Assert.True(v2Endpoint.Count == 1);
        Assert.True(staticEndpoint.Count == 1);
    }

    public void Dispose()
    {
    }
}