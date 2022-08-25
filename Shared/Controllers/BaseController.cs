using System;
using System.Linq;
using Shared.Controllers.Attributes;
using Shared.Logging.Interfaces;
using System.Reflection;

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
                var action = methodInfo.GetCustomAttribute<ActionAttribute>()?.Name;
                if (string.IsNullOrEmpty(action)) continue;

                try
                {
                    var value = (Func<string, string?>)methodInfo.CreateDelegate(typeof(Func<string, string?>), this);
                    controllerMethods.Add(action, value);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    throw new Exception($"Action method must return 'string?' and contain 'string' parameter");
                }
            }

            return controllerMethods;
        }
    }
}