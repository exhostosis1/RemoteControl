using RemoteControlApp.Web.Attributes;
using RemoteControlApp.Web.Controllers;
using Shared;
using Shared.Interfaces.Web;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;

namespace RemoteControlApp.Web.Middleware
{
    public class ApiMiddlewareV1 : IMiddleware
    {
        private readonly Dictionary<string, Dictionary<string, Func<string, string?>>> _methods = new();

        private const string ApiVersion = "v1";

        private Type GetDelegateType(Type returnType, Type[] paramTypes)
        {
            return returnType == typeof(void)
                ? Expression.GetActionType(paramTypes)
                : Expression.GetFuncType(paramTypes.Concat(new[] { returnType }).ToArray());
        }

        public ApiMiddlewareV1(IEnumerable<BaseController> controllers)
        {
            foreach (var controller in controllers)
            {
                var controllerKey = controller.GetType().GetCustomAttribute<ControllerAttribute>()?.Name;

                if (string.IsNullOrEmpty(controllerKey)) continue;

                var methods = controller.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.ReturnType == typeof(string) && x.GetParameters().Length == 1 && x.GetParameters().First().ParameterType == typeof(string)).ToArray();

                if (methods.Length == 0) continue;

                var controllerValue = new Dictionary<string, Func<string, string?>>();

                foreach (var methodInfo in methods)
                {
                    var action = methodInfo.GetCustomAttribute<ActionAttribute>()?.Name;
                    if (string.IsNullOrEmpty(action)) continue;

                    var value = methodInfo.CreateDelegate<Func<string, string?>>(controller);

                    controllerValue.Add(action, value);
                }

                if (controllerValue.Count > 0)
                {
                    _methods.Add(controllerKey, controllerValue);
                }
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
