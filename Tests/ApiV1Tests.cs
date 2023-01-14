﻿using Moq;
using Servers.Endpoints;
using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.DataObjects.Http;
using Shared.Logging.Interfaces;
using System.Net;

namespace Tests;

public class ApiV1Tests: IDisposable
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

        public ControllerMethods GetMethods()
        {
            return new ControllerMethods(new Dictionary<string, Func<string?, IActionResult>>
            {
                {
                    "actionone", ActionOne
                },
                {
                    "actiontwo", ActionTwo
                }
            });
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

        public ControllerMethods GetMethods()
        {
            return new ControllerMethods(new Dictionary<string, Func<string?, IActionResult>>
            {
                {
                    "actionthree", ActionThree
                },
                {
                    "actionfour", ActionFour
                }
            });
        }
    }

    [Fact]
    public void RequestTest()
    {
        var logger = Mock.Of<ILogger<ApiV1Endpoint>>();

        var controllers = new List<IApiController>
        {
            new FirstController(),
            new SecondController()
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