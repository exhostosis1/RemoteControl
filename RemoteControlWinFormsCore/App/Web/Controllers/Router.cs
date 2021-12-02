using System.Net;
using RemoteControl.App.Utility;

namespace RemoteControl.App.Web.Controllers
{
    internal static class Router
    {
        public static void ProcessRequest(HttpListenerContext context)
        {
            if (context == null) return;

            var path = context.Request.Url?.LocalPath;

            var (api, version, method, param) = path?.Split("/", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            Response response;

            if(api == "api" && version == "v1")
            {
                response = ApiControllerV1.ProcessRequest(method, param);
            }
            else
            {
                response = FileController.ProcessRequest(path);
            }

            context.Response.StatusCode = (int)response.StatusCode;
            context.Response.ContentType = response.ContentType;
            

            if (response.Payload.Length > 0)
            {
                context.Response.OutputStream.Write(response.Payload);
            }
        }
    }
}
