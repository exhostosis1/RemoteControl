using RemoteControlCore.Abstract;
using RemoteControlCore.Utility;
using System.Collections.Generic;
using System.Reflection;

namespace RemoteControlCore.Controllers
{
    internal partial class ApiController : AbstractController
    {
        private readonly Dictionary<string, MethodInfo> _methods;

        public override string ProcessApiRequest(string message)
        {
            var (methodName, param) = Strings.ParseAddresString(message);

            if (string.IsNullOrWhiteSpace(methodName) || !_methods.ContainsKey(methodName)) return null;

            var result = _methods[methodName].Invoke(this, new object[]{param});

            return (string) result;
        }
    }
}
