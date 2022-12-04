using Shared.DataObjects.Interfaces;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers.Middleware;

public class RoutingMiddleware: AbstractMiddleware
{
    private readonly IEnumerable<AbstractEndpoint> _endpoints;
    private const string ApiString = "/api/";

    public RoutingMiddleware(IEnumerable<AbstractEndpoint> endpoints, ILogger logger, HttpEventHandler? next = null) : base(logger, next)
    {
        _endpoints = endpoints;
    }

    public override void ProcessRequest(IContext context)
    {
        var path = context.Request.Path;

        if (path.Contains(ApiString))
        {
            var startIndex = path.IndexOf(ApiString, StringComparison.OrdinalIgnoreCase);

            if (startIndex != -1)
            {
                startIndex += ApiString.Length;

                var endIndex = path.IndexOf('/', startIndex + 1);

                if (endIndex != -1)
                {
                    var version = path[startIndex..endIndex].ToLower();

                    _endpoints.FirstOrDefault(x => x.ApiVersion == version)?.ProcessRequest(context);
                }
            }
                
        }
        else
        {
            _endpoints.FirstOrDefault(x => x.IsStaticFiles)?.ProcessRequest(context);
        }

        Next?.Invoke(context);
    }
}