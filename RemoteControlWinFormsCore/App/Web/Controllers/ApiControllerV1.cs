using RemoteControl.App.Control;
using RemoteControl.App.Enums;
using RemoteControl.App.Utility;
using System.Net;
using System.Text;

namespace RemoteControl.App.Web.Controllers
{
    internal static class ApiControllerV1
    {
        private static readonly Dictionary<string, Func<string, string?>> _methods = new()
        {
            { 
                "audio", Audio
            },
            {
                "keyboard", KeyboardKey
            },
            {
                "mouse", Mouse
            },
            {
                "text", TextInput
            },
            {
                "display", DisplayControl
            }
        };

        public static string? Audio(string param)
        {
            if (param == "init") return ControlFacade.GetVolume().ToString();

            if (!int.TryParse(param, out var result)) return null;

            result = result > 100 ? 100 : result < 0 ? 0 : result;

            ControlFacade.SetVolume(result);
            ControlFacade.Mute(result == 0);

            return null;
        }

        public static string? KeyboardKey(string param)
        {
            switch (param)
            {
                case "back":
                    ControlFacade.KeyboardKeyPress(KeysEnum.Back);
                    break;
                case "forth":
                    ControlFacade.KeyboardKeyPress(KeysEnum.Forth);
                    break;
                case "pause":
                    ControlFacade.KeyboardKeyPress(KeysEnum.Pause);
                    break;
                case "mediaback":
                    ControlFacade.KeyboardKeyPress(KeysEnum.MediaBack);
                    break;
                case "mediaforth":
                    ControlFacade.KeyboardKeyPress(KeysEnum.MediaForth);
                    break;
            }

            return null;
        }

        public static string? Mouse(string param)
        {
            switch (param)
            {
                case "left":
                    ControlFacade.MouseKeyPress(MouseKeysEnum.Left);
                    break;
                case "right":
                    ControlFacade.MouseKeyPress(MouseKeysEnum.Right);
                    break;
                case "middle":
                    ControlFacade.MouseKeyPress(MouseKeysEnum.Middle);
                    break;
                case "up":
                    ControlFacade.MouseWheel(true);
                    break;
                case "down":
                    ControlFacade.MouseWheel(false);
                    break;
                case "dragstart":
                    ControlFacade.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Down);
                    Task.Run(async () =>
                    {
                        await Task.Delay(5_000);
                        ControlFacade.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
                    });
                    break;
                case "dragstop":
                    ControlFacade.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
                    break;
                default:
                    if (Utils.TryGetCoords(param, out var x, out var y))
                    {
                        ControlFacade.MouseMove(x, y);
                    }
                    break;
            }

            return null;
        }

        public static string? TextInput(string param)
        {
            ControlFacade.TextInput(WebUtility.UrlDecode(param));
            ControlFacade.KeyboardKeyPress(KeysEnum.Enter);

            return null;
        }

        public static string? DisplayControl(string param)
        {
            switch (param)
            {
                case "darken":
                    ControlFacade.DisplayDarken();
                    break;
                default:
                    break;
            }

            return null;
        }

        public static Response ProcessRequest(string method, string param)
        {
            var response = new Response();

            if (!_methods.ContainsKey(method))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            var result = _methods[method].Invoke(param);

            if (!string.IsNullOrEmpty(result))
            {
                response.ContentType = "application/json";
                response.Payload = Encoding.UTF8.GetBytes(result);
            }

            return response;
        }
    }
}
