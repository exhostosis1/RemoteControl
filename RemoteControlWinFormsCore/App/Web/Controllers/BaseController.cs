using System.Net;

namespace RemoteControl.App.Web.Controllers
{
    internal static class BaseController
    {
        private static readonly HttpController _httpController = new();
        private static readonly ApiController _apiController = new();

        public static void ProcessRequest(HttpListenerContext context)
        {
            if (context == null) return;

            if(context.Request?.RawUrl?.Contains("api") ?? false)
            {
                _apiController.ProcessRequest(context);
            }
            else
            {
                _httpController.ProcessRequest(context);
            }
        }
    }
}
