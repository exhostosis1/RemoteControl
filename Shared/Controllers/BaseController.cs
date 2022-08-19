using System.Reflection;
using Shared.Controllers.Attributes;
using Shared.Logging.Interfaces;

namespace Shared.Controllers
{
    public abstract class BaseController
    {
        protected readonly ILogger Logger;

        protected BaseController(ILogger logger)
        {
            Logger = logger;
        }

        public ControllerMethods GetMethods()
        {
            var controllerMethods = new ControllerMethods();

            var type = GetType();

            var controllerKey = type.GetCustomAttribute<ControllerAttribute>()?.Name;

            if (string.IsNullOrEmpty(controllerKey)) return controllerMethods;

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(ActionAttribute))).ToArray();

            if (methods.Length == 0) return controllerMethods;

            foreach (var methodInfo in methods)
            {
                if (methodInfo.ReturnType != typeof(string) ||
                    methodInfo.GetParameters().Length != 1 ||
                    methodInfo.GetParameters().First().ParameterType != typeof(string))
                {
                    throw new Exception($"Action method must return 'string?' and contain 'string' parameter");
                }

                var action = methodInfo.GetCustomAttribute<ActionAttribute>()?.Name;
                if (string.IsNullOrEmpty(action)) continue;

                var value = methodInfo.CreateDelegate<Func<string, string?>>(this);

                controllerMethods.Add(action, value);
            }

            return controllerMethods;
        }
    }
}