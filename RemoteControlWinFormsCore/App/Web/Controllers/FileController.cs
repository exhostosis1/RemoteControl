using System.Net;

namespace RemoteControl.App.Web.Controllers
{
    internal static class FileController
    {
        private readonly static string ContentFolder = AppContext.BaseDirectory + "www";

        private readonly static Dictionary<string, string> _contentTypes = new()
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

            response.ContentType = _contentTypes.ContainsKey(extension) ? _contentTypes[extension] : "text/plain";

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
