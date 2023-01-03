using Shared.DataObjects.Interfaces;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using System.Text.RegularExpressions;

namespace Servers.Middleware;

public partial class RoutingMiddleware: AbstractMiddleware
{
    private readonly IEnumerable<AbstractApiEndpoint> _apiEndpoints;
    private readonly AbstractEndpoint _staticFilesEndpoint;
    private const string ApiString = "/api/";

    [GeneratedRegex($"(?<={ApiString})v\\d+")]
    private partial Regex ApiRegex();

    public RoutingMiddleware(IEnumerable<AbstractApiEndpoint> apiEndpoints, AbstractEndpoint staticFilesEndpoint, ILogger logger,
        HttpEventHandler? next = null) : base(logger, next)
    {
        _apiEndpoints = apiEndpoints;
        _staticFilesEndpoint = staticFilesEndpoint;
    }

    public override void ProcessRequest(IContext context)
    {
        var match = ApiRegex().Match(context.Request.Path);

        if (match.Success)
        {
            var endpoint = _apiEndpoints.FirstOrDefault(x => x.ApiVersion == match.Value);

            if (endpoint == null)
            {
                context.Response.StatusCode = HttpStatusCode.NotFound;
            }
            else
            {
                endpoint.ProcessRequest(context);   
            }
        }
        else
        {
            _staticFilesEndpoint.ProcessRequest(context);
        }

        Next?.Invoke(context);
    }
}