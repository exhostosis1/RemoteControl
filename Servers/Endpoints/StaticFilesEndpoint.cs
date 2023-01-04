using System.Net;
using Shared.DataObjects.Interfaces;
using Shared.Logging.Interfaces;
using Shared.Server;

namespace Servers.Endpoints;

public class StaticFilesEndpoint : AbstractEndpoint
{
    private readonly string _contentFolder;

    public StaticFilesEndpoint(ILogger logger, string directory = "www") : base(logger)
    {
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

    public override void ProcessRequest(IContext context)
    {
        var uriPath = context.Request.Path;

        Logger.LogInfo($"Processing file request {uriPath}");

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
            Logger.LogError($"File not found {path}");
            context.Response.StatusCode = HttpStatusCode.NotFound;
        }
    }
}