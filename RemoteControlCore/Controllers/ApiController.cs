using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WindowsInputLib;
using WindowsInputLib.Native;
using RemoteControlCore.Interfaces;
using RemoteControlCore.Utility;
using RemoteControlCore.Abstract;

namespace RemoteControlCore.Controllers
{
    internal class ApiController : AbstractController, IObserver<DeviceChangedArgs>
    {
        CoreAudioDevice _audioDevice;
        InputSimulator _inputsim;

        public ApiController()
        {
            _inputsim = new InputSimulator(); 
            var audioController = new CoreAudioController();

            Task.Run(async () => _audioDevice = await audioController.GetDefaultDeviceAsync(DeviceType.Playback, Role.Multimedia)).Wait();
            audioController.AudioDeviceChanged.Subscribe(this);
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
                    response = processAudio(value);
                    break;
                case "mouse":
                case "wheel":
                    processMouse(value);
                    break;
                case "keyboard":
                    processKeyboard(value);
                    break;
                case "text":
                    if (tryGetString(value, args.Request.ContentEncoding, out var str))
                    {
                        _inputsim.Keyboard.TextEntry(str);
                        _inputsim.Keyboard.KeyPress(VirtualKeyCode.Enter);
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

        private bool tryGetString(string input, Encoding encoding, out string output)
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

        private string processAudio(string value)
        {
            if (Int32.TryParse(value, out var result))
            {
                result = result > 100 ? 100 : result;
                result = result < 0 ? 0 : result;

                _audioDevice.Volume = result;
                _audioDevice.Mute(result == 0);
            }

            return _audioDevice.Volume.ToString();
        }
        private void processKeyboard(string value)
        {
            switch (value)
            {
                case "back":
                    _inputsim.Keyboard.KeyPress(VirtualKeyCode.Left);
                    break;
                case "forth":
                    _inputsim.Keyboard.KeyPress(VirtualKeyCode.Right);
                    break;
                case "pause":
                    _inputsim.Keyboard.KeyPress(VirtualKeyCode.MediaPlayPause);
                    break;
                default:
                    break;
            }
        }

        private void processMouse(string value)
        {
            switch (value)
            {
                case "1":
                    _inputsim.Mouse.MouseButtonClick(MouseButton.LeftButton);
                    break;
                case "2":
                    _inputsim.Mouse.MouseButtonClick(MouseButton.RightButton);
                    break;
                case "3":
                    _inputsim.Mouse.MouseButtonClick(MouseButton.MiddleButton);
                    break;
                case "up":
                    _inputsim.Mouse.VerticalScroll(1);
                    break;
                case "down":
                    _inputsim.Mouse.VerticalScroll(-1);
                    break;
                case "dragstart":
                    _inputsim.Mouse.MouseButtonDown(MouseButton.LeftButton);
                    Task.Run(async () =>
                    {
                        await Task.Delay(5_000);
                        _inputsim.Mouse.MouseButtonUp(MouseButton.LeftButton);
                    });
                    break;
                case "dragstop":
                    _inputsim.Mouse.MouseButtonUp(MouseButton.LeftButton);
                    break;
                default:
                    if (Point.TryConvert(value.Replace("\"", ""), out var diff))
                    {
                        _inputsim.Mouse.MoveMouseBy(diff.X, diff.Y);
                    }
                    break;
            }
        }

        public void OnNext(DeviceChangedArgs value)
        {
            if(value.ChangedType == DeviceChangedType.DefaultChanged && value.Device.DeviceType == DeviceType.Playback)
            {
                _audioDevice = (CoreAudioDevice)value.Device;
            }
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
