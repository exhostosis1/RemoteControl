using Moq;
using Servers.Endpoints;
using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.DataObjects.Http;
using Shared.Logging.Interfaces;
using System.Net;

namespace Tests;

public class ApiV1Tests: IDisposable
{
    public ApiV1Tests()
    {

    }

    private class FirstController : BaseApiController
    {
        public int Onecount;
        public int Twocount;

        public FirstController(ILogger logger) : base(logger)
        {
        }

        public IActionResult ActionOne(string _)
        {
            Onecount++;
            return Ok();
        }

        public IActionResult ActionTwo(string _)
        {
            Twocount++;
            return Error("test error");
        }
    }

    private class SecondController : BaseApiController
    {
        public int Threecount;
        public int Fourcount;


        public SecondController(ILogger logger) : base(logger)
        {
        }

        public IActionResult ActionThree(string _)
        {
            Threecount++;
            return Json("new");
        }

        public IActionResult ActionFour(string _)
        {
            Fourcount++;
            return Text("text");
        }
    }

    [Fact]
    public void RequestTest()
    {
        var logger = Mock.Of<ILogger<ApiV1Endpoint>>();

        var controllers = new List<BaseApiController>
        {
            new FirstController(logger),
            new SecondController(logger)
        };

        var apiEndpoint = new ApiV1Endpoint(controllers, logger);

        var context = new Context("/api/v1/first/actionone");

        apiEndpoint.ProcessRequest(context);

        Assert.True(context.Response.StatusCode == HttpStatusCode.OK);

        context = new Context("/api/v1/first/actiontwo");
        apiEndpoint.ProcessRequest(context);
        Assert.True(context.Response is {StatusCode: HttpStatusCode.InternalServerError, ContentType: "text/plain"});

        context = new Context("/api/v1/second/actionthree");
        apiEndpoint.ProcessRequest(context);
        Assert.True(context.Response is {StatusCode: HttpStatusCode.OK, ContentType: "application/json"});

        context = new Context("/api/v1/second/actionfour");
        apiEndpoint.ProcessRequest(context);
        Assert.True(context.Response is {StatusCode: HttpStatusCode.OK, ContentType: "text/plain"});

        Assert.True((controllers[0] as FirstController)?.Onecount == 1);
        Assert.True((controllers[0] as FirstController)?.Twocount == 1);
        Assert.True((controllers[1] as SecondController)?.Threecount == 1);
        Assert.True((controllers[1] as SecondController)?.Fourcount == 1);

        context = new Context("/api/v1/second/actionfive");
        apiEndpoint.ProcessRequest(context);
        Assert.True(context.Response.StatusCode == HttpStatusCode.NotFound);
    }

    public void Dispose()
    {

    }
}