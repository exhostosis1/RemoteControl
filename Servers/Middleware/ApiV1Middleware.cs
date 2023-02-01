﻿using Shared;
using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.DataObjects.Web;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using System.Text;

namespace Servers.Middleware;

public class ApiV1Middleware : AbstractMiddleware<WebContext>, IWebMiddleware
{
    private readonly ControllersWithMethods _controllers;
    private readonly ILogger<ApiV1Middleware> _logger;
    public string ApiVersion => "v1";

    public ApiV1Middleware(IEnumerable<IApiController> controllers, ILogger<ApiV1Middleware> logger,
        IWebMiddleware? next = null) : base(next)
    {
        _logger = logger;
        _controllers = controllers.GetControllersWithMethods();
    }

    private static byte[] GetBytes(string? input) => Encoding.UTF8.GetBytes(input ?? string.Empty);

    public override void ProcessRequest(WebContext context)
    {
        if (!Utils.TryGetApiVersion(context.WebRequest.Path, out var version) || version != ApiVersion)
        {
            Next?.ProcessRequest(context);
            return;
        }

        _logger.LogInfo($"Processing api request {context.WebRequest.Path}");

        if (!Utils.TryParsePath(context.WebRequest.Path, out var controller, out var action, out var param) || !_controllers.ContainsKey(controller)
            || !_controllers[controller].ContainsKey(action))
        {

            context.WebResponse.StatusCode = HttpStatusCode.NotFound;
            _logger.LogError("Api method not found");
            return;
        }

        try
        {
            var result = _controllers[controller][action](param);

            context.WebResponse.StatusCode = result.StatusCode;
            context.WebResponse.Payload = GetBytes(result.Result);

            if (result is JsonResult)
                context.WebResponse.ContentType = "application/json";
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            context.WebResponse.StatusCode = HttpStatusCode.InternalServerError;
            context.WebResponse.Payload = Encoding.UTF8.GetBytes(e.Message);
        }
    }
}