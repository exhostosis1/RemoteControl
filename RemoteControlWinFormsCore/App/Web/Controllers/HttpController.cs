using RemoteControl.App.Web.Interfaces;

namespace RemoteControl.App.Web.Controllers
{
    internal class HttpController : AbstractController
    {
        private readonly string ContentFolder = AppContext.BaseDirectory + "www";

        private readonly Dictionary<string, string> _contentTypes = new Dictionary<string, string>()
        {
            { ".html", "text/html" },
            { ".htm", "text/html" },
            { ".ico", "image/x-icon" },
            { ".js", "text/javascript" },
            { ".mjs", "text/javascript" },
            { ".css", "text/css" }
        };

        public override void ProcessRequest(IHttpRequestArgs context)
        {
            var path = ContentFolder + context?.Request?.Url?.LocalPath;

            if (context?.Request?.Url?.LocalPath == "/")
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
