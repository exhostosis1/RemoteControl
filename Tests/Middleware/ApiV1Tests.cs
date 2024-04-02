using Moq;
using Servers.Middleware;
using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.DataObjects.Web;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;

namespace UnitTests.Middleware;

public class ApiV1Tests : IDisposable
{
    private class FirstController : IApiController
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult ActionOne(string? _)
        {
            return new OkResult();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult ActionTwo(string? _)
        {
            return new ErrorResult("test error");
        }
    }

    private class SecondController : IApiController
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult ActionThree(string? _)
        {
            return new JsonResult("new");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult ActionFour(string? _)
        {
            return new TextResult("text");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult ErrorAction(string? _)
        {
            throw new Exception("test exception");
        }
    }

    private readonly ApiV1Middleware _middleware;
    private readonly ILogger _logger;

    public ApiV1Tests()
    {
        var controllers = new List<IApiController>
        {
            new FirstController(),
            new SecondController()
        };

        _logger = Mock.Of<ILogger>();

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
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}