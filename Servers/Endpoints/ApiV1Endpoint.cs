using Shared;
using Shared.Controllers;
using Shared.Controllers.Results;
using Shared.DataObjects.Interfaces;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using System.Text;

namespace Servers.Endpoints;

public class ApiV1Endpoint : AbstractEndpoint
{
    private readonly ControllersWithMethods _controllers;

    public ApiV1Endpoint(IEnumerable<BaseController> controllers, ILogger logger) : base(logger)
    {
        _controllers = controllers.GetControllersWithMethods();

        ApiVersion = "v1";
        IsStaticFiles = false;
    }

    private static byte[] GetBytes(string? input) => Encoding.UTF8.GetBytes(input ?? string.Empty);

    public override void ProcessRequest(IContext context)
    {
        if (!context.Request.Path.TryParsePath(out var controller, out var action, out var param) || !_controllers.ContainsKey(controller)
            || !_controllers[controller].ContainsKey(action))
        {
            context.Response.StatusCode = HttpStatusCode.NotFound;
            return;
        }

        try
        {
            var result = _controllers[controller][action](param);

            context.Response.StatusCode = result.StatusCode;
            context.Response.Payload = GetBytes(result.Result);

            switch (result)
            {
                case ErrorResult:
                case StringResult:
                case OkResult:
                    context.Response.ContentType = "text/plain";
                    break;
                case JsonResult:
                    context.Response.ContentType = "application/json";
                    break;
            }
        }
        catch (Exception e)
        {
            context.Response.StatusCode = HttpStatusCode.InternalServerError;
            context.Response.ContentType = "text/plain";
            context.Response.Payload = Encoding.UTF8.GetBytes(e.Message);
        }
    }
}