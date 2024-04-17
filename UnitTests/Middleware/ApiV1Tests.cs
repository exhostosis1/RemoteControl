using MainApp.Servers.ApiControllers;
using MainApp.Servers.DataObjects;
using MainApp.Servers.Middleware;
using MainApp.Servers.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Middleware;

public class ApiV1Tests : IDisposable
{
    private class FirstController : BaseApiController
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

    private class SecondController : BaseApiController
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
        var controllers = new List<BaseApiController>
        {
            new FirstController(),
            new SecondController()
        };

        var logger = Mock.Of<ILogger>();

        _middleware = new ApiV1Middleware(controllers, logger);
    }

    [Theory]
    [InlineData("/api/v1/first/actionone", RequestStatus.Ok, "")]
    [InlineData("/api/v1/first/actiontwo", RequestStatus.Error, "test error")]
    [InlineData("/api/v1/second/actionthree", RequestStatus.Json, "\"new\"")]
    [InlineData("/api/v1/second/actionfour", RequestStatus.Text, "text")]
    [InlineData("/api/v1/second/actionfive", RequestStatus.NotFound, "")]
    public async Task RequestTest(string path, RequestStatus expectedCode, string expectedResult)
    {
        var context = new RequestContext
        {
            Path = path
        };

        await _middleware.ProcessRequestAsync(context, null!);

        Assert.True(context.Status == expectedCode && context.Reply == expectedResult);
    }

    [Fact]
    public async Task RequestNextTest()
    {
        var count = 0;

        var context = new RequestContext
        { 
            Path = "/api/v2/first/actionone"
        };
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
        var context = new RequestContext
        {
            Path = "/api/v1/second/erroraction"
        };
        await _middleware.ProcessRequestAsync(context, null!);

        Assert.Equal(RequestStatus.Error, context.Status);

        var res = context.Reply;
        Assert.Equal("test exception", res);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}