using RemoteControlCore.Abstract;
using RemoteControlCore.Interfaces;
using RemoteControlCore.Utility;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RemoteControlCore.Controllers
{
    internal partial class ApiController : AbstractController
    {
        private readonly Dictionary<string, MethodInfo> _methods;

        public override void ProcessRequest(IHttpRequestArgs args)
        {
            var (methodName, param) = Strings.ParseAddresString(args.Request.RawUrl);

            if (string.IsNullOrWhiteSpace(methodName)) return;

            if (!_methods.ContainsKey(methodName)) return;

            var result = _methods[methodName].Invoke(this, new object[]{param});

            if (param == "init")
            {
                using (var sw = new StreamWriter(args.Response.OutputStream))
                {
                    sw.Write(result);
                }
            }
        }
    }
}
