using Shared;
using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using System.Text;
using Shared.DataObjects.Http;

namespace Servers.Endpoints;

public class ApiV1Middleware : AbstractMiddleware<HttpContext>
{
    private readonly ControllersWithMethods _controllers;
    private readonly ILogger<ApiV1Middleware> _logger;
    public string ApiVersion => "v1";

    public ApiV1Middleware(IEnumerable<IApiController> controllers, ILogger<ApiV1Middleware> logger,
        AbstractMiddleware<HttpContext>? next = null) : base(next)
    {
        _logger = logger;
        _controllers = controllers.GetControllersWithMethods();
    }

    private static byte[] GetBytes(string? input) => Encoding.UTF8.GetBytes(input ?? string.Empty);

    public override void ProcessRequest(HttpContext context)
    {
        if (!Utils.TryGetApiVersion(context.Request.Path, out var version) || version != ApiVersion)
        {
            Next?.ProcessRequest(context);
            return;
        }

        _logger.LogInfo($"Processing api request {context.Request.Path}");

        if (!Utils.TryParsePath(context.Request.Path, out var controller, out var action, out var param) || !_controllers.ContainsKey(controller)
            || !_controllers[controller].ContainsKey(action))
        {

            context.Response.StatusCode = HttpStatusCode.NotFound;
            _logger.LogError("Api method not found");
            return;
        }

        try
        {
            var result = _controllers[controller][action](param);

            context.Response.StatusCode = result.StatusCode;
            context.Response.Payload = GetBytes(result.Result);

            if (result is JsonResult)
                context.Response.ContentType = "application/json";
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);

            context.Response.StatusCode = HttpStatusCode.InternalServerError;
            context.Response.Payload = Encoding.UTF8.GetBytes(e.Message);
        }
    }
}