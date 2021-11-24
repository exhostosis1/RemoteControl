using RemoteControl.App.Control;
using RemoteControl.App.Enums;
using RemoteControl.App.Utility;
using System.Net;

namespace RemoteControl.App.Web.Controllers
{
    internal static class ApiController
    {
        private static readonly Dictionary<string, Func<string, string?>> _methods = new()
        {
            { 
                "audio", param =>
                {                    
                    if (param == "init") return ControlFacade.GetVolume().ToString();

                    if (!int.TryParse(param, out var result)) return null;

                    result = result > 100 ? 100 : result < 0 ? 0 : result;

                    ControlFacade.SetVolume(result);
                    ControlFacade.Mute(result == 0);

                    return null;
                } 
            },
            {
                "keyboard", param =>
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
            },
            {
                "mouse", param =>
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
            },
            {
                "text", param =>
                {
                    ControlFacade.TextInput(WebUtility.UrlDecode(param));
                    ControlFacade.KeyboardKeyPress(KeysEnum.Enter);

                    return null;
                }
            },
            {
                "display", param =>
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
            }
        };

        public static void ProcessRequest(HttpListenerContext context)
        {
            var (methodName, param) = Utils.ParseAddresString(context.Request?.RawUrl ?? string.Empty);

            if (!_methods.ContainsKey(methodName))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var result = _methods[methodName].Invoke(param);
            
            if (!string.IsNullOrEmpty(result))
            {
                using var sw = new StreamWriter(context.Response.OutputStream);
                sw.Write(result);
            }
        }
    }
}
