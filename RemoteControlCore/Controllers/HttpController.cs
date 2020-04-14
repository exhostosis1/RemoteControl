using RemoteControlCore.Interfaces;
using System.Collections.Generic;
using System.IO;
using RemoteControlCore.Abstract;

namespace RemoteControlCore.Controllers
{
    internal class HttpController : AbstractController
    {
        private readonly string _ContentFolder = "www";

        private readonly Dictionary<string, string> ContentTypes = new Dictionary<string, string>()
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
            var path = _ContentFolder + context.Request.Url.LocalPath;

            if (context.Request.Url.LocalPath == "/")
            {
                path += context.Request.UserAgent.Contains("SM-R800") ? "index-simple.html" : "index.html";
            }

            var extension = Path.GetExtension(path);

            context.Response.ContentType = ContentTypes.ContainsKey(extension) ? ContentTypes[extension] : "text/plain";

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
