using Microsoft.Extensions.Logging;
using Shared;
using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.DataObjects.Web;
using Shared.Server;
using System.Net;
using System.Text;
using Shared.DataObjects;

namespace Servers.Middleware;

public class ApiV1Middleware(IEnumerable<IApiController> controllers, ILogger logger) : IMiddleware
{
    private readonly ControllersWithMethods _controllers = controllers.GetControllersWithMethods();
    public static string ApiVersion => "v1";

    private static byte[] GetBytes(string? input) => Encoding.UTF8.GetBytes(input ?? string.Empty);
    
    public async Task ProcessRequestAsync(IContext contextParam, Func<IContext, Task> next)
    {
        var context = (WebContext)contextParam;

        if (!Utils.TryGetApiVersion(context.WebRequest.Path, out var version) || version != ApiVersion)
        {
            await next(contextParam);
            return;
        }

        logger.LogInformation("Processing api request {path}", context.WebRequest.Path);

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
            logger.LogError("{message}", e.Message);

            context.WebResponse.StatusCode = HttpStatusCode.InternalServerError;
            context.WebResponse.Payload = Encoding.UTF8.GetBytes(e.Message);
        }
    }
}