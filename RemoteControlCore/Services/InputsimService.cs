using RemoteControlCore.Enums;
using RemoteControlCore.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsInputLib;
using WindowsInputLib.Native;

namespace RemoteControlCore.Services
{
    internal class InputsimService : IInputService
    {
        private readonly InputSimulator _inputSim;

        public InputsimService()
        {
            _inputSim = new InputSimulator();
        }

        readonly Dictionary<KeysEnum, VirtualKeyCode> keyboardKeys = new Dictionary<KeysEnum, VirtualKeyCode>()
        {
            {
                KeysEnum.Back,
                VirtualKeyCode.Left
            },
            {
                KeysEnum.Enter,
                VirtualKeyCode.Enter
            },
            {
                KeysEnum.Forth,
                VirtualKeyCode.Right
            },
            {
                KeysEnum.Pause,
                VirtualKeyCode.MediaPlayPause
            }
        };

        readonly Dictionary<MouseKeysEnum, MouseButton> mouseKeys = new Dictionary<MouseKeysEnum, MouseButton>()
        {
            {
                MouseKeysEnum.Left,
                MouseButton.LeftButton
            },
            {
                MouseKeysEnum.Right,
                MouseButton.RightButton
            },
            {
                MouseKeysEnum.Middle,
                MouseButton.MiddleButton
            }
        };

        public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click)
        {
            var inputKey = keyboardKeys[key];

            switch (mode)
            {
                case KeyPressMode.Click:
                    _inputSim.Keyboard.KeyPress(inputKey);
                    break;
                case KeyPressMode.Up:
                    _inputSim.Keyboard.KeyUp(inputKey);
                    break;
                case KeyPressMode.Down:
                    _inputSim.Keyboard.KeyDown(inputKey);
                    break;
                default:
                    break;
            }
        }

        public void MouseKeyPress(MouseKeysEnum key = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click)
        {
            var inputKey = mouseKeys[key];

            switch (mode)
            {
                case KeyPressMode.Click:
                    _inputSim.Mouse.MouseButtonClick(inputKey);
                    break;
                case KeyPressMode.Down:
                    _inputSim.Mouse.MouseButtonDown(inputKey);
                    Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        _inputSim.Mouse.MouseButtonUp(inputKey);
                    });
                    break;
                case KeyPressMode.Up:
                    _inputSim.Mouse.MouseButtonUp(inputKey);
                    break;
            }
        }

        public void MouseMove(ICoordinates coords)
        {
            _inputSim.Mouse.MoveMouseBy(coords.X, coords.Y);
        }

        public void MouseWheel(bool up)
        {
            _inputSim.Mouse.VerticalScroll(up ? 1 : -1);
        }

        public void TextInput(string text)
        {
            _inputSim.Keyboard.TextEntry(text);
        }
    }
}
