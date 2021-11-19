using RemoteControl.App.Web.Interfaces;
using RemoteControl.App.Utility;

namespace RemoteControl.App.Web.Controllers
{
    internal partial class ApiController : AbstractController
    {
        public override void ProcessRequest(IHttpRequestArgs context)
        {
            var (methodName, param) = Strings.ParseAddresString(context?.Request?.RawUrl ?? string.Empty);

            switch (methodName)
            {
                case "audio":
                    var result = ProcessAudio(param);

                    if (param == "init")
                    {
                        using var sw = new StreamWriter(context?.Response?.OutputStream ?? new MemoryStream());
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
            }
        }
    }
}
