using Shared;
using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.DataObjects.Web;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using System.Text;

namespace Servers.Middleware;

public class ApiV1Middleware(IEnumerable<IApiController> controllers, ILogger<ApiV1Middleware> logger) : IWebMiddleware
{
    private readonly ControllersWithMethods _controllers = controllers.GetControllersWithMethods();
    private readonly ILogger<ApiV1Middleware> _logger = logger;
    public static string ApiVersion => "v1";

    private static byte[] GetBytes(string? input) => Encoding.UTF8.GetBytes(input ?? string.Empty);

    public event EventHandler<WebContext>? OnNext;

    public void ProcessRequest(object? _, WebContext context)
    {
        if (!Utils.TryGetApiVersion(context.WebRequest.Path, out var version) || version != ApiVersion)
        {
            OnNext?.Invoke(null, context);
            return;
        }

        _logger.LogInfo($"Processing api request {context.WebRequest.Path}");

        if (!Utils.TryParsePath(context.WebRequest.Path, out var controllerName, out var actionName, out var param) 
            || !_controllers.TryGetValue(controllerName, out ControllerMethods? controller) 
            || !controller.TryGetValue(actionName, out Func<string?, IActionResult>? action))
        {

            context.WebResponse.StatusCode = HttpStatusCode.NotFound;
            _logger.LogError("Api method not found");
            return;
        }

        try
        {
            var result = action(param);

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