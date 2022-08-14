using RemoteControl.App.Interfaces.Control;
using RemoteControl.App.Interfaces.Web;
using RemoteControl.App.Web.Controllers;
using System.Net;
using System.Reflection;
using System.Text;
using Windows.Media.Audio;

namespace RemoteControl.App.Web.Middleware
{
    internal class InternalMethodInfo
    {
        public object Controller { get; set; }
        public MethodInfo Method { get; set; }
        public bool HasParameter { get; set; }
        public bool HasValue { get; set; }
    }

    internal class ApiMiddlewareV1 : IMiddleware
    {
        private readonly Dictionary<string, Dictionary<string, InternalMethodInfo>> _methods = new();
        private readonly Dictionary<Type, object> _deps = new();

        private const string ApiVersion = "v1";

        public ApiMiddlewareV1(IDisplayControl displayControl, IKeyboardControl keyboardControl,
            IMouseControl mouseControl, IAudioControl audioControl)
        {
            _deps.Add(typeof(IDisplayControl), displayControl);
            _deps.Add(typeof(IKeyboardControl), keyboardControl);
            _deps.Add(typeof(IMouseControl), mouseControl);
            _deps.Add(typeof(IAudioControl), audioControl);

            var controllers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass && t.BaseType == typeof(BaseController)).ToList();

            foreach (var controllerType in controllers)
            {
                var controllerKey = controllerType.GetControllerName();

                if (string.IsNullOrEmpty(controllerKey)) continue;

                var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                if (methods.Length == 0) continue;
                var constructor = controllerType.GetConstructors().First();
                var constructorParams = constructor.GetParameters().Select(x => _deps[x.ParameterType]).ToArray();

                var controller = constructor.Invoke(constructorParams);

                var controllerValue = new Dictionary<string, InternalMethodInfo>();

                foreach (var methodInfo in methods)
                {
                    var action = methodInfo.GetActionName();
                    if (string.IsNullOrEmpty(action)) continue;

                    var value = new InternalMethodInfo
                    {
                        Method = methodInfo,
                        HasValue = methodInfo.ReturnType != typeof(void),
                        HasParameter = methodInfo.GetParameters().Length > 0,
                        Controller = controller
                    };

                    controllerValue.Add(action, value);
                }

                if (controllerValue.Any())
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

            var actionInfo = _methods[controller][action];

            var result = actionInfo.Method.Invoke(actionInfo.Controller, actionInfo.HasParameter ? new object?[] { param } : null);

            if (actionInfo.HasValue && result is string res)
            {
                context.Response.ContentType = "application/json";
                context.Response.Payload = Encoding.UTF8.GetBytes(res);
            }
        }
    }
}
