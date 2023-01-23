using Moq;
using Servers.Middleware;
using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.DataObjects.Http;
using Shared.Logging.Interfaces;
using System.Net;

namespace Tests.Endpoints;

public class ApiV1Tests : IDisposable
{
    private class FirstController : IApiController
    {
        public int Onecount;
        public int Twocount;

        public IActionResult ActionOne(string? _)
        {
            Onecount++;
            return new OkResult();
        }

        public IActionResult ActionTwo(string? _)
        {
            Twocount++;
            return new ErrorResult("test error");
        }
    }

    private class SecondController : IApiController
    {
        public int Threecount;
        public int Fourcount;

        public IActionResult ActionThree(string? _)
        {
            Threecount++;
            return new JsonResult("new");
        }

        public IActionResult ActionFour(string? _)
        {
            Fourcount++;
            return new StringResult("text");
        }
    }

    private class LocalResponse : HttpContextResponse
    {
        public override void Close()
        {

        }
    }

    [Fact]
    public void RequestTest()
    {
        var logger = Mock.Of<ILogger<ApiV1Middleware>>();

        var controllers = new List<IApiController>
        {
            new FirstController(),
            new SecondController()
        };

        var apiEndpoint = new ApiV1Middleware(controllers, logger);

        var response = Mock.Of<HttpContextResponse>();

        var context = new HttpContext(new HttpContextRequest("/api/v1/first/actionone"), response);

        apiEndpoint.ProcessRequest(context);

        Assert.True(context.HttpResponse.StatusCode == HttpStatusCode.OK);

        context = new HttpContext(new HttpContextRequest("/api/v1/first/actiontwo"), response);
        apiEndpoint.ProcessRequest(context);
        Assert.True(context.HttpResponse is { StatusCode: HttpStatusCode.InternalServerError, ContentType: "text/plain" });

        context = new HttpContext(new HttpContextRequest("/api/v1/second/actionthree"), response);
        apiEndpoint.ProcessRequest(context);
        Assert.True(context.HttpResponse is { StatusCode: HttpStatusCode.OK, ContentType: "application/json" });

        context = new HttpContext(new HttpContextRequest("/api/v1/second/actionfour"), response);
        apiEndpoint.ProcessRequest(context);
        Assert.True(context.HttpResponse is { StatusCode: HttpStatusCode.OK, ContentType: "text/plain" });

        Assert.True((controllers[0] as FirstController)?.Onecount == 1);
        Assert.True((controllers[0] as FirstController)?.Twocount == 1);
        Assert.True((controllers[1] as SecondController)?.Threecount == 1);
        Assert.True((controllers[1] as SecondController)?.Fourcount == 1);

        context = new HttpContext(new HttpContextRequest("/api/v1/second/actionfive"), response);
        apiEndpoint.ProcessRequest(context);
        Assert.True(context.HttpResponse.StatusCode == HttpStatusCode.NotFound);
    }

    public void Dispose()
    {

    }
}