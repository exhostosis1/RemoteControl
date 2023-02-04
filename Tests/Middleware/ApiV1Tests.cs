using System.Net;
using System.Text;
using Moq;
using Servers.Middleware;
using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.DataObjects.Web;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace UnitTests.Middleware;

public class ApiV1Tests : IDisposable
{
    private class FirstController : IApiController
    {
        public IActionResult ActionOne(string? _)
        {
            return new OkResult();
        }

        public IActionResult ActionTwo(string? _)
        {
            return new ErrorResult("test error");
        }
    }

    private class SecondController : IApiController
    {
        public IActionResult ActionThree(string? _)
        {
            return new JsonResult("new");
        }

        public IActionResult ActionFour(string? _)
        {
            return new TextResult("text");
        }

        public IActionResult ErrorAction(string? _)
        {
            throw new Exception("test exception");
        }
    }

    private readonly ApiV1Middleware _middleware;
    private readonly ILogger<ApiV1Middleware> _logger;

    public ApiV1Tests()
    {
        var controllers = new List<IApiController>
        {
            new FirstController(),
            new SecondController()
        };

        _logger = Mock.Of<ILogger<ApiV1Middleware>>();

        _middleware = new ApiV1Middleware(controllers, _logger);
    }

    [Theory]
    [InlineData("/api/v1/first/actionone", HttpStatusCode.OK, "text/plain", "")]
    [InlineData("/api/v1/first/actiontwo", HttpStatusCode.InternalServerError, "text/plain", "test error")]
    [InlineData("/api/v1/second/actionthree", HttpStatusCode.OK, "application/json", "\"new\"")]
    [InlineData("/api/v1/second/actionfour", HttpStatusCode.OK, "text/plain", "text")]
    [InlineData("/api/v1/second/actionfive", HttpStatusCode.NotFound, "text/plain", "")]
    public void RequestTest(string path, HttpStatusCode expectedCode, string expectedContentType, string expectedResult)
    {
        var context = new WebContext(new WebContextRequest(path), Mock.Of<WebContextResponse>());
        _middleware.ProcessRequest(null, context);

        Assert.True(context.WebResponse.StatusCode == expectedCode
                    && context.WebResponse.ContentType == expectedContentType
                    && Encoding.UTF8.GetString(context.WebResponse.Payload) == expectedResult);

        if(expectedCode == HttpStatusCode.NotFound)
            Mock.Get(_logger).Verify(x => x.LogError("Api method not found"), Times.Once);
    }

    [Fact]
    public void RequestNextText()
    {
        var count = 0;

        _middleware.OnNext += (_, _) => count++;

        var context = new WebContext(new WebContextRequest("/api/v2/first/actionone"), Mock.Of<WebContextResponse>());
        _middleware.ProcessRequest(null, context);

        Assert.True(count == 1);
    }

    [Fact]
    public void ErrorTest()
    {
        var context = new WebContext(new WebContextRequest("/api/v1/second/erroraction"), Mock.Of<WebContextResponse>());
        _middleware.ProcessRequest(null, context);

        Assert.True(context.WebResponse.StatusCode == HttpStatusCode.InternalServerError &&
                    Encoding.UTF8.GetString(context.WebResponse.Payload) == "test exception");

        Mock.Get(_logger).Verify(x => x.LogError("test exception"), Times.Once);
    }

    public void Dispose()
    {

    }
}