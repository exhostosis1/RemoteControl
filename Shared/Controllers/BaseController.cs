using Shared.Logging.Interfaces;
using System;
using System.Linq;
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

            var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).ToArray();

            if (methods.Length == 0) return controllerMethods;

            foreach (var methodInfo in methods)
            {
                var action = methodInfo.GetActionName();
                if (string.IsNullOrEmpty(action)) continue;

                try
                {
                    var value = methodInfo.CreateDelegate<Func<string, string?>>(this);
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