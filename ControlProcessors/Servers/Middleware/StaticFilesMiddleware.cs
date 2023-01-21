using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using Shared.DataObjects.Http;

namespace Servers.Middleware;

public class StaticFilesMiddleware : AbstractMiddleware<HttpContext>
{
    private readonly string _contentFolder;
    private readonly ILogger<StaticFilesMiddleware> _logger;

    public StaticFilesMiddleware(ILogger<StaticFilesMiddleware> logger, string directory = "www")
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

    public override void ProcessRequest(HttpContext context)
    {
        var uriPath = context.HttpRequest.Path;

        _logger.LogInfo($"Processing file request {uriPath}");

        if (uriPath.Contains(".."))
        {
            context.HttpResponse.StatusCode = HttpStatusCode.NotFound;
            return;
        }

        var path = _contentFolder + uriPath;

        if (string.IsNullOrEmpty(uriPath) || uriPath == "/")
        {
            path += "index.html";
        }

        var extension = Path.GetExtension(path);

        context.HttpResponse.ContentType = ContentTypes.TryGetValue(extension, out var value) ? value : "text/plain";

        if (File.Exists(path))
        {
            context.HttpResponse.Payload = File.ReadAllBytes(path);
        }
        else
        {
            _logger.LogError($"File not found {path}");
            context.HttpResponse.StatusCode = HttpStatusCode.NotFound;
        }
    }
}