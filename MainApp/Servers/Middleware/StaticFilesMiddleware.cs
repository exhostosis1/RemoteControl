using System.Net;
using MainApp.Servers.DataObjects;
using Microsoft.Extensions.Logging;

namespace MainApp.Servers.Middleware;

internal class StaticFilesMiddleware(ILogger logger, string directory = "www") : IMiddleware
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
        var uriPath = context.Path;

        logger.LogInformation("Processing file request {uriPath}", uriPath);

        if (uriPath.Contains(".."))
        {
            context.Status = RequestStatus.NotFound;
            return;
        }

        var path = Path.Combine(_contentFolder, uriPath.Replace("/", "").Replace("\\", ""));

        if (string.IsNullOrEmpty(uriPath) || uriPath == "/")
        {
            path = Path.Combine(path, "index.html");
        }

        var extension = Path.GetExtension(path);

        context.Status = RequestStatus.Custom;

        if (context.OriginalRequest is not HttpListenerContext original) return;

        original.Response.ContentType = ContentTypes.GetValueOrDefault(extension, "text/plain");

        if (File.Exists(path))
        {
            await original.Response.OutputStream.WriteAsync(await File.ReadAllBytesAsync(path));
            original.Response.StatusCode = (int)HttpStatusCode.OK;
        }
        else
        {
            logger.LogError("File not found {path}", path);
            original.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}