using RemoteControl.App.Interfaces.Web;
using System.Net;

namespace RemoteControl.App.Web.Middleware
{
    internal class FileMiddleware : IMiddleware
    {
        private static readonly string ContentFolder = AppContext.BaseDirectory + "www";

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
