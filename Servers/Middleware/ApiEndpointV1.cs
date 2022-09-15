using Shared;
using Shared.Controllers;
using Shared.DataObjects.Interfaces;
using Shared.Server.Interfaces;
using System.Net;
using System.Text;

namespace Servers.Middleware
{
    public class ApiEndpointV1 : IMiddleware
    {
        private readonly Dictionary<string, ControllerMethods> _methods = new();

        private const string ApiVersion = "v1";

        public ApiEndpointV1(IEnumerable<BaseController> controllers)
        {
            foreach (var controller in controllers)
            {
                var controllerName = controller.GetControllerName();

                if(string.IsNullOrEmpty(controllerName)) continue;

                _methods.Add(controllerName, controller.GetMethods());
            }
        }

        public void ProcessRequest(IContext context)
        {
            if(!context.Request.Path.TryParsePath(ApiVersion, out var controller, out var action, out var param)
               || !_methods.ContainsKey(controller) || !_methods[controller].ContainsKey(action))
            {
                context.Response.StatusCode = HttpStatusCode.NotFound;
                return;
            }

            try
            {
                var result = _methods[controller][action](param ?? "");

                if (!string.IsNullOrEmpty(result))
                {
                    context.Response.ContentType = "application/json";
                    context.Response.Payload = Encoding.UTF8.GetBytes(result);
                }
            }
            catch (Exception e)
            {
                context.Response.StatusCode = HttpStatusCode.InternalServerError;
                context.Response.Payload = Encoding.UTF8.GetBytes(e.Message);
            }
        }
    }
}
