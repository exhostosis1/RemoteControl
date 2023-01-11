using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using Shared.DataObjects.Http;

namespace Servers.Endpoints;

public class StaticFilesEndpoint : AbstractEndpoint
{
    private readonly string _contentFolder;
    private readonly ILogger<StaticFilesEndpoint> _logger;

    public StaticFilesEndpoint(ILogger<StaticFilesEndpoint> logger, string directory = "www")
    {
        _logger = logger;
        _contentFolder = AppContext.BaseDirectory + directory;
    }

    private static readonly Dictionary<string, string> ContentTypes = new()
    {
        { ".html", "text/html" },
        { ".htm", "text/html" },
        { ".ico", "image/x-icon" },
        { ".js", "text/javascript" },
        { ".mjs", "text/javascript" },
        { ".css", "text/css" }
    };

    public override void ProcessRequest(Context context)
    {
        var uriPath = context.Request.Path;

        _logger.LogInfo($"Processing file request {uriPath}");

        if (uriPath.Contains(".."))
        {
            context.Response.StatusCode = HttpStatusCode.NotFound;
            return;
        }

        var path = _contentFolder + uriPath;

        if (string.IsNullOrEmpty(uriPath) || uriPath == "/")
        {
            path += "index.html";
        }

        var extension = Path.GetExtension(path);

        context.Response.ContentType = ContentTypes.TryGetValue(extension, out var value) ? value : "text/plain";

        if (File.Exists(path))
        {
            context.Response.Payload = File.ReadAllBytes(path);
        }
        else
        {
            _logger.LogError($"File not found {path}");
            context.Response.StatusCode = HttpStatusCode.NotFound;
        }
    }
}