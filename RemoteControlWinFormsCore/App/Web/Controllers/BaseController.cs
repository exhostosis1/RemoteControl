using System.Net;

namespace RemoteControl.App.Web.Controllers
{
    internal static class BaseController
    {
        public static void ProcessRequest(HttpListenerContext context)
        {
            if (context == null) return;

            if(context.Request?.RawUrl?.Contains("api") ?? false)
            {
                ApiController.ProcessRequest(context);
            }
            else
            {
                HttpController.ProcessRequest(context);
            }
        }
    }
}
