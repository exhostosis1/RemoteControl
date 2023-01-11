using Shared;
using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using System.Text;
using Shared.DataObjects.Http;

namespace Servers.Endpoints;

public class ApiV1Endpoint : AbstractApiEndpoint
{
    private readonly ControllersWithMethods _controllers;
    private readonly ILogger<ApiV1Endpoint> _logger;

    public ApiV1Endpoint(IEnumerable<BaseApiController> controllers, ILogger<ApiV1Endpoint> logger)
    {
        _logger = logger;
        _controllers = controllers.GetControllersWithMethods();

        ApiVersion = "v1";
    }

    private static byte[] GetBytes(string? input) => Encoding.UTF8.GetBytes(input ?? string.Empty);

    public override void ProcessRequest(Context context)
    {
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