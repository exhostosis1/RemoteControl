using System.Net;

namespace RemoteControl.App.Web.Controllers
{
    internal static class HttpController
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

        public static void ProcessRequest(HttpListenerContext context)
        {
            var path = ContentFolder + context.Request.Url?.LocalPath;

            if (context.Request.Url?.LocalPath == "/")
            {
                path += "index.html";
            }

            var extension = Path.GetExtension(path);

            if (context == null) return;

            context.Response.ContentType = _contentTypes.ContainsKey(extension) ? _contentTypes[extension] : "text/plain";

            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }
    }
}
