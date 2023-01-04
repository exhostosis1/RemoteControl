using Shared;
using Shared.Controllers;
using Shared.Controllers.Results;
using Shared.DataObjects.Interfaces;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using System.Text;

namespace Servers.Endpoints;

public class ApiV1Endpoint : AbstractApiEndpoint
{
    private readonly ControllersWithMethods _controllers;

    public ApiV1Endpoint(IEnumerable<BaseController> controllers, ILogger logger) : base(logger)
    {
        _controllers = controllers.GetControllersWithMethods();

        ApiVersion = "v1";
    }

    private static byte[] GetBytes(string? input) => Encoding.UTF8.GetBytes(input ?? string.Empty);

    public override void ProcessRequest(IContext context)
    {
        if (!Utils.TryParsePath(context.Request.Path, out var controller, out var action, out var param) || !_controllers.ContainsKey(controller)
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

            if (result is JsonResult)
                context.Response.ContentType = "application/json";
        }
        catch (Exception e)
        {
            context.Response.StatusCode = HttpStatusCode.InternalServerError;
            context.Response.Payload = Encoding.UTF8.GetBytes(e.Message);
        }
    }
}