using DependencyFactory;
using RemoteControlCore.Abstract;
using RemoteControlCore.Enums;
using RemoteControlCore.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlCore.Controllers
{
    internal class ApiController : AbstractController
    {
        readonly IAudioService _audioService;
        readonly IInputService _inputService;
        readonly ICoordinates _point;

        public ApiController()
        {
            _inputService = Factory.GetInstance<IInputService>();
            _audioService = Factory.GetInstance<IAudioService>();
            _point = Factory.GetInstance<ICoordinates>();
        }
        
        public override void ProcessRequest(IHttpRequestArgs args)
        {
            args.Response.StatusCode = 200;

            var mode = args.Request.QueryString["mode"];
            var value = args.Request.QueryString["value"];
            var response = "";

            switch (mode)
            {
                case "audio":
                    response = ProcessAudio(value);
                    break;
                case "mouse":
                case "wheel":
                    ProcessMouse(value);
                    break;
                case "keyboard":
                    ProcessKeyboard(value);
                    break;
                case "text":
                    if (TryGetString(value, args.Request.ContentEncoding, out var str))
                    {
                        ProcessText(str);
                    }
                    break;
                default:
                    break;
            }

            if(value == "init")
            {
                using (var sw = new StreamWriter(args.Response.OutputStream))
                {
                    sw.Write(response);
                }
            }
        }

        private bool TryGetString(string input, Encoding encoding, out string output)
        {
            output = "";
            if (string.IsNullOrWhiteSpace(input)) return false;

            if(encoding == Encoding.UTF8)
            {
                output = input;
                return true;
            }

            try
            {
                output = Encoding.UTF8.GetString(encoding.GetBytes(input));
            }
            catch
            {
                return false;
            }

            return true;
        }

        private string ProcessAudio(string value)
        {
            if (Int32.TryParse(value, out var result))
            {
                result = result > 100 ? 100 : result;
                result = result < 0 ? 0 : result;

                _audioService.SetVolume(result);
                _audioService.Mute(result == 0);
            }

            return _audioService.GetVolume();
        }
        private void ProcessKeyboard(string value)
        {
            switch (value)
            {
                case "back":
                    _inputService.KeyPress(KeysEnum.Back);
                    break;
                case "forth":
                    _inputService.KeyPress(KeysEnum.Forth);
                    break;
                case "pause":
                    _inputService.KeyPress(KeysEnum.Pause);
                    break;
                default:
                    break;
            }
        }

        private void ProcessText(string text)
        {
            _inputService.TextInput(text);
            _inputService.KeyPress(KeysEnum.Enter);
        }

        private void ProcessMouse(string value)
        {
            switch (value)
            {
                case "1":
                    _inputService.MouseKeyPress();
                    break;
                case "2":
                    _inputService.MouseKeyPress(MouseKeysEnum.Right);
                    break;
                case "3":
                    _inputService.MouseKeyPress(MouseKeysEnum.Middle);
                    break;
                case "up":
                    _inputService.MouseWheel(true);
                    break;
                case "down":
                    _inputService.MouseWheel(false);
                    break;
                case "dragstart":
                    _inputService.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Down);
                    Task.Run(async () =>
                    {
                        await Task.Delay(5_000);
                        _inputService.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
                    });
                    break;
                case "dragstop":
                    _inputService.MouseKeyPress(MouseKeysEnum.Left, KeyPressMode.Up);
                    break;
                default:
                    if (_point.TrySetCoords(value.Replace("\"", "")))
                    {
                        _inputService.MouseMove(_point);
                    }
                    break;
            }
        }
    }
}
