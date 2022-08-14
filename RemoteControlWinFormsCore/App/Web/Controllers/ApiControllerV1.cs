using RemoteControl.App.Control;
using RemoteControl.App.Enums;
using RemoteControl.App.Interfaces.Web;
using RemoteControl.App.Utility;
using System.Net;
using System.Text;
using System.Text.Json;

namespace RemoteControl.App.Web.Controllers
{
    internal class ApiControllerV1: IController
    {
        private readonly ControlFacade _controlFacade;

        private readonly Dictionary<string, Func<string, string?>> _methods;

        public ApiControllerV1(ControlFacade facade)
        {
            _controlFacade = facade;

            _methods = new()
            {
                {
                    "audio", Audio
                },
                {
                    "audiodevice", AudioDevice
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
                },
                {
                    "mousemove", MouseMove
                }
            };
        }

        public void ProcessRequest(IContext context)
        {
            var (method, param) = context.Request.Path.Split('/');

            if (!_methods.ContainsKey(method))
            {
                context.Response.StatusCode = HttpStatusCode.BadRequest;
                return;
            }

            var result = _methods[method].Invoke(param);

            if (!string.IsNullOrEmpty(result))
            {
                context.Response.ContentType = "application/json";
                context.Response.Payload = Encoding.UTF8.GetBytes(result);
            }
        }

        private string? AudioDevice(string param)
        {
            if (param == "get") return JsonSerializer.Serialize(_controlFacade.GetDevices());

            if (Guid.TryParse(param, out var guid)) _controlFacade.SetDevice(guid);

            return null;
        }

        private string? Audio(string param)
        {
            if (param == "init") return _controlFacade.GetVolume().ToString();

            if (!int.TryParse(param, out var result)) return null;

            result = result > 100 ? 100 : result < 0 ? 0 : result;

            _controlFacade.SetVolume(result);
            _controlFacade.Mute(result == 0);

            return null;
        }

        private string? KeyboardKey(string param)
        {
            switch (param)
            {
                case "back":
                    _controlFacade.KeyboardKeyPress(KeysEnum.Back);
                    break;
                case "forth":
                    _controlFacade.KeyboardKeyPress(KeysEnum.Forth);
                    break;
                case "pause":
                    _controlFacade.KeyboardKeyPress(KeysEnum.Pause);
                    break;
                case "mediaback":
                    _controlFacade.KeyboardKeyPress(KeysEnum.MediaBack);
                    break;
                case "mediaforth":
                    _controlFacade.KeyboardKeyPress(KeysEnum.MediaForth);
                    break;
            }

            return null;
        }

        private string? Mouse(string param)
        {
            switch (param)
            {
                case "left":
                    _controlFacade.MouseKeyPress(MouseKeysEnum.Left);
                    break;
                case "right":
                    _controlFacade.MouseKeyPress(MouseKeysEnum.Right);
                    break;
                case "middle":
                    _controlFacade.MouseKeyPress(MouseKeysEnum.Middle);
                    break;
                case "up":
                    _controlFacade.MouseWheel(true);
                    break;
                case "down":
                    _controlFacade.MouseWheel(false);
                    break;
                case "dragstart":
                    _controlFacade.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Down);
                    Task.Run(async () =>
                    {
                        await Task.Delay(5_000);
                        _controlFacade.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
                    });
                    break;
                case "dragstop":
                    _controlFacade.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
                    break;
            }

            return null;
        }

        private string? MouseMove(string param)
        {
            if (Utils.TryGetCoords(param, out var x, out var y))
            {
                _controlFacade.MouseMove(x, y);
            }

            return null;
        }

        private string? TextInput(string param)
        {
            _controlFacade.TextInput(WebUtility.UrlDecode(param));
            _controlFacade.KeyboardKeyPress(KeysEnum.Enter);

            return null;
        }

        private string? DisplayControl(string param)
        {
            switch (param)
            {
                case "darken":
                    _controlFacade.DisplayDarken();
                    break;
            }

            return null;
        }
    }
}
