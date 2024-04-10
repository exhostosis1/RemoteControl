using Microsoft.Extensions.Logging;
using System.Net;
using Servers.DataObjects;

namespace Servers.Middleware;

public class StaticFilesMiddleware(ILogger logger, string directory = "www") : IMiddleware
{
    private readonly string _contentFolder = Path.Combine(AppContext.BaseDirectory, directory);

    private static readonly Dictionary<string, string> ContentTypes = new()
    {
        { ".html", "text/html" },
        { ".htm", "text/html" },
        { ".ico", "image/x-icon" },
        { ".js", "text/javascript" },
        { ".mjs", "text/javascript" },
        { ".css", "text/css" }
    };
    
    public async Task ProcessRequestAsync(RequestContext context, RequestDelegate _)
    {
        var uriPath = context.Input.Path;

        logger.LogInformation("Processing file request {uriPath}", uriPath);

        if (uriPath.Contains(".."))
        {
            context.Output.StatusCode = HttpStatusCode.NotFound;
            return;
        }

        var path = Path.Combine(_contentFolder, uriPath.Replace("/", "").Replace("\\", ""));

        if (string.IsNullOrEmpty(uriPath) || uriPath == "/")
        {
            path = Path.Combine(path, "index.html");
        }

        var extension = Path.GetExtension(path);

        context.Output.ContentType = ContentTypes.GetValueOrDefault(extension, "text/plain");

        if (File.Exists(path))
        {
            context.Output.Payload = await File.ReadAllBytesAsync(path);
        }
        else
        {
            logger.LogError("File not found {path}", path);
            context.Output.StatusCode = HttpStatusCode.NotFound;
        }
    }
}