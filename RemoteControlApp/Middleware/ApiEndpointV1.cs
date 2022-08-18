using Shared;
using Shared.Interfaces;
using Shared.Interfaces.Web;
using System.Net;
using System.Reflection;
using System.Text;
using WebApiProvider.Attributes;
using WebApiProvider.Controllers;

namespace RemoteControlApp.Middleware
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ApiEndpointV1 : IMiddleware
    {
        private readonly Dictionary<string, Dictionary<string, Func<string, string?>>> _methods = new();

        private const string ApiVersion = "v1";

        public ApiEndpointV1(HttpEventHandler _, IContainer container)
        {
            var controllers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x =>
                x.IsClass && x.IsAssignableTo(typeof(BaseController)));

            foreach (var controllerType in controllers)
            {
                var controllerKey = controllerType.GetCustomAttribute<ControllerAttribute>()?.Name;

                if (string.IsNullOrEmpty(controllerKey)) continue;

                var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.ReturnType == typeof(string) && x.GetParameters().Length == 1 && x.GetParameters().First().ParameterType == typeof(string)).ToArray();

                if (methods.Length == 0) continue;

                var controllerInstance = container.GetUnregistered(controllerType);

                var controllerValue = new Dictionary<string, Func<string, string?>>();

                foreach (var methodInfo in methods)
                {
                    var action = methodInfo.GetCustomAttribute<ActionAttribute>()?.Name;
                    if (string.IsNullOrEmpty(action)) continue;

                    var value = methodInfo.CreateDelegate<Func<string, string?>>(controllerInstance);

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

    public static partial class AppExtensions
    {
        public static IRemoteControlApp UseApiV1Enpoint(this IRemoteControlApp app)
        {
            app.UseMiddleware<ApiEndpointV1>();
            return app;
        }
    }
}
