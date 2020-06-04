using DependencyFactory;
using RemoteControlCore.Attributes;
using RemoteControlCore.Enums;
using RemoteControlCore.Interfaces;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Local

namespace RemoteControlCore.Controllers
{
    internal partial class ApiController
    {
        private readonly IAudioService _audioService;
        private readonly IInputService _inputService;
        private readonly ICoordinates _point;

        public ApiController()
        {
            _inputService = Factory.GetInstance<IInputService>();
            _audioService = Factory.GetInstance<IAudioService>();
            _point = Factory.GetInstance<ICoordinates>();

            _methods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(x => x.GetCustomAttribute<RouteAttribute>() != null)
                .ToDictionary(x => x.GetCustomAttribute<RouteAttribute>().MethodName);
        }

        [Route("audio")]
        private string ProcessAudio(string value)
        {
            if (!int.TryParse(value, out var result)) return _audioService.GetVolume();

            result = result > 100 ? 100 : result;
            result = result < 0 ? 0 : result;

            _audioService.SetVolume(result);

            _audioService.Mute(result == 0);

            return _audioService.GetVolume();
        }

        [Route("keyboard")]
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
                case "mediaback":
                    _inputService.KeyPress(KeysEnum.MediaBack);
                    break;
                case "mediaforth":
                    _inputService.KeyPress(KeysEnum.MediaForth);
                    break;
            }
        }

        [Route("text")]
        private void ProcessText(string text)
        {
            _inputService.TextInput(WebUtility.UrlDecode(text));
            _inputService.KeyPress(KeysEnum.Enter);
        }

        [Route("mouse")]
        private void ProcessMouse(string value)
        {
            switch (value)
            {
                case "left":
                    _inputService.MouseKeyPress();
                    break;
                case "right":
                    _inputService.MouseKeyPress(MouseKeysEnum.Right);
                    break;
                case "middle":
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
