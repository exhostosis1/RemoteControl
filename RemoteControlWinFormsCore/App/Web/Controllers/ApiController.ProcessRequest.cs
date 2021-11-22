using RemoteControl.App.Utility;
using System.Net;

namespace RemoteControl.App.Web.Controllers
{
    internal partial class ApiController
    {
        public void ProcessRequest(HttpListenerContext context)
        {
            var (methodName, param) = Strings.ParseAddresString(context.Request?.RawUrl ?? string.Empty);

            switch (methodName)
            {
                case "audio":
                    var result = ProcessAudio(param);

                    if (param == "init")
                    {
                        using var sw = new StreamWriter(context.Response.OutputStream);
                        sw.Write(result);
                    }

                    break;
                case "mouse":
                    ProcessMouse(param);
                    break;
                case "text":
                    ProcessText(param);
                    break;
                case "keyboard":
                    ProcessKeyboard(param);
                    break;
                default:
                    context.Response.StatusCode = 400;
                    break;
            }
        }
    }
}
