using System.Net;

namespace RemoteControl.App.Web.Controllers
{
    internal static class FileController
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

        public static Response ProcessRequest(string? uriPath)
        {
            var response = new Response();

            var path = ContentFolder + uriPath;

            if (string.IsNullOrEmpty(uriPath) || uriPath == "/")
            {
                path += "index.html";
            }

            var extension = Path.GetExtension(path);

            response.ContentType = ContentTypes.ContainsKey(extension) ? ContentTypes[extension] : "text/plain";

            if (File.Exists(path))
            {
                response.Payload = File.ReadAllBytes(path);
            }
            else
            {
                response.StatusCode = HttpStatusCode.NotFound;
            }

            return response;
        }
    }
}
