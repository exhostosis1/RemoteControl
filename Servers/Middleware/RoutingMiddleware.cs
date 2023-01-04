using Shared.DataObjects.Interfaces;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using Shared;

namespace Servers.Middleware;

public partial class RoutingMiddleware: AbstractMiddleware
{
    private readonly IEnumerable<AbstractApiEndpoint> _apiEndpoints;
    private readonly AbstractEndpoint _staticFilesEndpoint;

    public RoutingMiddleware(IEnumerable<AbstractApiEndpoint> apiEndpoints, AbstractEndpoint staticFilesEndpoint, ILogger logger,
        HttpEventHandler? next = null) : base(logger, next)
    {
        _apiEndpoints = apiEndpoints;
        _staticFilesEndpoint = staticFilesEndpoint;
    }

    public override void ProcessRequest(IContext context)
    {
        Logger.LogInfo($"Routing request {context.Request.Path}");

        if (Utils.TryGetApiVersion(context.Request.Path, out var version))
        {
            var endpoint = _apiEndpoints.FirstOrDefault(x => x.ApiVersion == version);

            if (endpoint == null)
            {
                Logger.LogError($"Api endpoint not found for request {context.Request.Path}");
                context.Response.StatusCode = HttpStatusCode.NotFound;
            }
            else
            {
                Logger.LogInfo($"Passing request to api endpoint version {version}");
                endpoint.ProcessRequest(context);   
            }
        }
        else
        {
            Logger.LogInfo("Passing request to static files endpoint");
            _staticFilesEndpoint.ProcessRequest(context);
        }

        Next?.Invoke(context);
    }
}