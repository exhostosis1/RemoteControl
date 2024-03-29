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

        logger.LogInfo($"Processing api request {context.WebRequest.Path}");

        if (!Utils.TryParsePath(context.WebRequest.Path, out var controllerName, out var actionName, out var param) 
            || !_controllers.TryGetValue(controllerName, out var controller) 
            || !controller.TryGetValue(actionName, out var action))
        {

            context.WebResponse.StatusCode = HttpStatusCode.NotFound;
            logger.LogError("Api method not found");
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
            logger.LogError(e.Message);

            context.WebResponse.StatusCode = HttpStatusCode.InternalServerError;
            context.WebResponse.Payload = Encoding.UTF8.GetBytes(e.Message);
        }
    }
}