using Shared.DataObjects.Web;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;

namespace Servers.Middleware;

public class StaticFilesMiddleware : IWebMiddleware
{
    private readonly string _contentFolder;
    private readonly ILogger<StaticFilesMiddleware> _logger;

    public StaticFilesMiddleware(ILogger<StaticFilesMiddleware> logger, string directory = "www")
    {
        _logger = logger;
        _contentFolder = Path.Combine(AppContext.BaseDirectory, directory);
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

    public event EventHandler<WebContext>? OnNext;

    public void ProcessRequest(object? _, WebContext context)
    {
        var uriPath = context.WebRequest.Path;

        _logger.LogInfo($"Processing file request {uriPath}");

        if (uriPath.Contains(".."))
        {
            context.WebResponse.StatusCode = HttpStatusCode.NotFound;
            return;
        }

        var path = Path.Combine(_contentFolder, uriPath.Replace("/", "").Replace("\\", ""));

        if (string.IsNullOrEmpty(uriPath) || uriPath == "/")
        {
            path = Path.Combine(path, "index.html");
        }

        var extension = Path.GetExtension(path);

        context.WebResponse.ContentType = ContentTypes.TryGetValue(extension, out var value) ? value : "text/plain";

        if (File.Exists(path))
        {
            context.WebResponse.Payload = File.ReadAllBytes(path);
        }
        else
        {
            _logger.LogError($"File not found {path}");
            context.WebResponse.StatusCode = HttpStatusCode.NotFound;
        }
    }
}