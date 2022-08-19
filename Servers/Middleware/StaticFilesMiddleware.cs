using System.Net;
using Shared.DataObjects.Interfaces;
using Shared.Server;
using Shared.Server.Interfaces;

namespace Servers.Middleware
{
    public class StaticFilesMiddleware : IMiddleware
    {
        private static readonly string ContentFolder = AppContext.BaseDirectory + "www";
        private readonly HttpEventHandler _next;

        public StaticFilesMiddleware(HttpEventHandler next)
        {
            _next = next;
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

        public void ProcessRequest(IContext context)
        {
            var uriPath = context.Request.Path;

            if (uriPath.Contains("/api/"))
            {
                _next(context);
                return;
            }

            if (uriPath.Contains(".."))
            {
                context.Response.StatusCode = HttpStatusCode.NotFound;
                return;
            }

            var path = ContentFolder + uriPath;

            if (string.IsNullOrEmpty(uriPath) || uriPath == "/")
            {
                path += "index.html";
            }

            var extension = Path.GetExtension(path);

            context.Response.ContentType = ContentTypes.ContainsKey(extension) ? ContentTypes[extension] : "text/plain";

            if (File.Exists(path))
            {
                context.Response.Payload = File.ReadAllBytes(path);
            }
            else
            {
                context.Response.StatusCode = HttpStatusCode.NotFound;
            }
        }
    }
}
