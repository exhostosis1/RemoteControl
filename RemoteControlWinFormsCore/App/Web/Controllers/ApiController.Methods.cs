using RemoteControl.App.Control;
using RemoteControl.App.Enums;
using RemoteControl.App.Utility;
using System.Net;

namespace RemoteControl.App.Web.Controllers
{
    internal partial class ApiController
    {
        private readonly ControlFacade _control = new();
        private readonly MyPoint _point = new();

        private int ProcessAudio(string value)
        {
            if (value == "init") return _control.GetVolume();

            if (!int.TryParse(value, out var result)) return 0;

            result = result > 100 ? 100 : result < 0 ? 0 : result;

            _control.SetVolume(result);

            _control.Mute(result == 0);

            return 0;
        }

        private void ProcessKeyboard(string value)
        {
            switch (value)
            {
                case "back":
                    _control.KeyboardKeyPress(KeysEnum.Back);
                    break;
                case "forth":
                    _control.KeyboardKeyPress(KeysEnum.Forth);
                    break;
                case "pause":
                    _control.KeyboardKeyPress(KeysEnum.Pause);
                    break;
                case "mediaback":
                    _control.KeyboardKeyPress(KeysEnum.MediaBack); 
                    break;
                case "mediaforth":
                    _control.KeyboardKeyPress(KeysEnum.MediaForth);
                    break;
            }
        }

        private void ProcessText(string text)
        {
            _control.TextInput(text);
            _control.KeyboardKeyPress(KeysEnum.Enter);
        }
        
        private void ProcessMouse(string value)
        {
            switch (value)
            {
                case "left":
                    _control.MouseKeyPress(MouseKeysEnum.Left);
                    break;
                case "right":
                    _control.MouseKeyPress(MouseKeysEnum.Right);
                    break;
                case "middle":
                    _control.MouseKeyPress(MouseKeysEnum.Middle);
                    break;
                case "up":
                    _control.MouseWheel(true);
                    break;
                case "down":
                    _control.MouseWheel(false);
                    break;
                case "dragstart":
                    _control.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Down);
                    Task.Run(async () =>
                    {
                        await Task.Delay(5_000);
                        _control.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
                    });
                    break;
                case "dragstop":
                    _control.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
                    break;
                default:
                    if (_point.TrySetCoords(WebUtility.UrlDecode(value).Replace("\"", "")))
                    {
                        _control.MouseMove(_point);
                    }
                    break;
            }
        }
    }
}
