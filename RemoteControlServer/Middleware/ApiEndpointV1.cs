using Shared;
using Shared.Controllers;
using Shared.Controllers.Attributes;
using Shared.DataObjects.Interfaces;
using Shared.Server.Interfaces;
using System.Net;
using System.Reflection;
using System.Text;

namespace Server.Middleware
{
    public class ApiEndpointV1 : IMiddleware
    {
        private readonly Dictionary<string, ControllerMethods> _methods = new();

        private const string ApiVersion = "v1";

        public ApiEndpointV1(IEnumerable<BaseController> controllers)
        {
            foreach (var controller in controllers)
            {
                var controllerName = controller.GetType().GetCustomAttribute<ControllerAttribute>()?.Name;

                if(string.IsNullOrEmpty(controllerName)) continue;

                _methods.Add(controllerName, controller.GetMethods());
            }
        }

        public void ProcessRequest(IContext context)
        {
            var (controller, action, param) = context.Request.Path.ParsePath(ApiVersion);

            if (!_methods.ContainsKey(controller) || !_methods[controller].ContainsKey(action))
            {
                context.Response.StatusCode = HttpStatusCode.NotFound;
                return;
            }

            var result = _methods[controller][action](param);

            if (!string.IsNullOrEmpty(result))
            {
                context.Response.ContentType = "application/json";
                context.Response.Payload = Encoding.UTF8.GetBytes(result);
            }
        }
    }
}
