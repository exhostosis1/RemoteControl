using System.IO;
using RemoteControlCore.Abstract;
using RemoteControlCore.Attributes;
using RemoteControlCore.Interfaces;
using RemoteControlCore.Utility;
using System.Linq;
using System.Reflection;

namespace RemoteControlCore.Controllers
{
    internal partial class ApiController : AbstractController
    {
        public override void ProcessRequest(IHttpRequestArgs args)
        {
            var (methodName, param) = Strings.ParseAddresString(args.Request.Url.LocalPath);

            if (string.IsNullOrWhiteSpace(methodName)) return;

            var method = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(x => (x.GetCustomAttribute(typeof(RouteAttribute)) as RouteAttribute)?.MethodName == methodName);

            if (method == null) return;

            var result = method.Invoke(this, new object[]{param});

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
