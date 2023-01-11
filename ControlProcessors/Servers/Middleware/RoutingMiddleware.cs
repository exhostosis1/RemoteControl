using Shared;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using Shared.DataObjects.Http;

namespace Servers.Middleware;

public partial class RoutingMiddleware: AbstractMiddleware
{
    private readonly IEnumerable<AbstractApiEndpoint> _apiEndpoints;
    private readonly AbstractEndpoint _staticFilesEndpoint;
    private readonly ILogger<RoutingMiddleware> _logger;

    public RoutingMiddleware(IEnumerable<AbstractApiEndpoint> apiEndpoints, AbstractEndpoint staticFilesEndpoint, ILogger<RoutingMiddleware> logger,
        HttpEventHandler? next = null) : base(next)
    {
        _logger = logger;
        _apiEndpoints = apiEndpoints;
        _staticFilesEndpoint = staticFilesEndpoint;
    }

    public override void ProcessRequest(Context context)
    {
        _logger.LogInfo($"Routing request {context.Request.Path}");

        if (Utils.TryGetApiVersion(context.Request.Path, out var version))
        {
            var endpoint = _apiEndpoints.FirstOrDefault(x => x.ApiVersion == version);

            if (endpoint == null)
            {
                _logger.LogError($"Api endpoint not found for request {context.Request.Path}");
                context.Response.StatusCode = HttpStatusCode.NotFound;
            }
            else
            {
                _logger.LogInfo($"Passing request to api endpoint version {version}");
                endpoint.ProcessRequest(context);   
            }
        }
        else
        {
            _logger.LogInfo("Passing request to static files endpoint");
            _staticFilesEndpoint.ProcessRequest(context);
        }

        Next?.Invoke(context);
    }
}