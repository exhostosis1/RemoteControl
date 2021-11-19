using RemoteControl.Core.Abstract;
using RemoteControl.Core.Interfaces;
using RemoteControl.Core.Utility;

namespace RemoteControl.Core.Controllers
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
