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
        public IActionResult ActionOne()
        {
            return new OkResult();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult ActionTwo()
        {
            return new ErrorResult("test error");
        }
    }

    private class SecondController : IApiController
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult ActionThree()
        {
            return new JsonResult("new");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult ActionFour()
        {
            return new TextResult("text");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        public IActionResult ErrorAction()
        {
            throw new Exception("test exception");
        }
    }

    private readonly ApiV1Middleware _middleware;

    public ApiV1Tests()
    {
        var controllers = new List<IApiController>
        {
            new FirstController(),
            new SecondController()
        };

        var logger = Mock.Of<ILogger>();

        _middleware = new ApiV1Middleware(controllers, logger);
    }

    [Theory]
    [InlineData("/api/v1/first/actionone", HttpStatusCode.OK, "text/plain", "")]
    [InlineData("/api/v1/first/actiontwo", HttpStatusCode.InternalServerError, "text/plain", "test error")]
    [InlineData("/api/v1/second/actionthree", HttpStatusCode.OK, "application/json", "\"new\"")]
    [InlineData("/api/v1/second/actionfour", HttpStatusCode.OK, "text/plain", "text")]
    [InlineData("/api/v1/second/actionfive", HttpStatusCode.NotFound, "text/plain", "")]
    public async Task RequestTest(string path, HttpStatusCode expectedCode, string expectedContentType, string expectedResult)
    {
        var context = new WebContext(new WebContextRequest(path), Mock.Of<WebContextResponse>());
        await _middleware.ProcessRequestAsync(context, null!);

        Assert.True(context.WebResponse.StatusCode == expectedCode
                    && context.WebResponse.ContentType == expectedContentType
                    && Encoding.UTF8.GetString(context.WebResponse.Payload) == expectedResult);
    }

    [Fact]
    public async Task RequestNextTest()
    {
        var count = 0;

        var context = new WebContext(new WebContextRequest("/api/v2/first/actionone"), Mock.Of<WebContextResponse>());
        await _middleware.ProcessRequestAsync(context, _ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task ErrorTest()
    {
        var context = new WebContext(new WebContextRequest("/api/v1/second/erroraction"), Mock.Of<WebContextResponse>());
        await _middleware.ProcessRequestAsync(context, null!);

        Assert.Equal(HttpStatusCode.InternalServerError, context.WebResponse.StatusCode);

        var res = Encoding.UTF8.GetString(context.WebResponse.Payload);
        Assert.Equal("test exception", res);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}