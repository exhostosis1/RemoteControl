using RemoteControl.App.Control;
using RemoteControl.App.Enums;
using RemoteControl.App.Utility;
using System.Net;

namespace RemoteControl.App.Web.Controllers
{
    internal static partial class ApiController
    {
        private static int ProcessAudio(string value)
        {
            if (value == "init") return ControlFacade.GetVolume();

            if (!int.TryParse(value, out var result)) return 0;

            result = result > 100 ? 100 : result < 0 ? 0 : result;

            ControlFacade.SetVolume(result);

            ControlFacade.Mute(result == 0);

            return 0;
        }

        private static void ProcessKeyboard(string value)
        {
            switch (value)
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
        }

        private static void ProcessText(string text)
        {
            ControlFacade.TextInput(WebUtility.UrlDecode(text));
            ControlFacade.KeyboardKeyPress(KeysEnum.Enter);
        }
        
        private static void ProcessMouse(string value)
        {
            switch (value)
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
                    if (Utils.TryGetCoords(value, out var x, out var y))
                    {
                        ControlFacade.MouseMove(x, y);
                    }
                    break;
            }
        }
    }
}
