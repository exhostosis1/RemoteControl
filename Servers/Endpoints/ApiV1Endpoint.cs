using Shared;
using Shared.Controllers;
using Shared.DataObjects.Interfaces;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using System.Text;

namespace Servers.Endpoints;

public class ApiV1Endpoint : AbstractEndpoint
{
    private readonly ControllersWithMethods _methods;

    public ApiV1Endpoint(IEnumerable<BaseController> controllers, ILogger logger) : base(logger)
    {
        _methods = Utils.GetControllersMethods(controllers);

        ApiVersion = "v1";
        IsStaticFiles = false;
    }

    public override void ProcessRequest(IContext context)
    {
        if (!context.Request.Path.TryParsePath(out var controller, out var action, out var param))
        {
            context.Response.StatusCode = HttpStatusCode.NotFound;
            return;
        }

        try
        {
            var result = _methods[controller][action](param);

            if (!string.IsNullOrEmpty(result))
            {
                context.Response.ContentType = "application/json";
                context.Response.Payload = Encoding.UTF8.GetBytes(result);
            }
        }
        catch (Exception e)
        {
            context.Response.StatusCode = HttpStatusCode.InternalServerError;
            context.Response.Payload = Encoding.UTF8.GetBytes(e.Message);
        }
    }
}