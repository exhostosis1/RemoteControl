using RemoteControl.App.Enums;
using RemoteControl.App.Control.Interfaces;
using RemoteControl.App.Utility;
using WindowsInputLib;
using WindowsInputLib.Native;

namespace RemoteControl.App.Control.Wrappers
{
    internal class WindowsInputLibWrapper : IControlInput
    {
        private readonly InputSimulator _inputSim = new();
        private static readonly Dictionary<KeysEnum, VirtualKeyCode> _keyboardKeys = new()
        {
            { KeysEnum.Enter, VirtualKeyCode.Enter },
            { KeysEnum.Forth, VirtualKeyCode.Right },
            { KeysEnum.Back, VirtualKeyCode.Left },
            { KeysEnum.MediaForth, VirtualKeyCode.MediaNextTrack },
            { KeysEnum.MediaBack, VirtualKeyCode.MediaPrevTrack },
            { KeysEnum.Pause, VirtualKeyCode.MediaPlayPause },
        };
        private static readonly Dictionary<MouseKeysEnum, MouseButton> _mouseKeys = new()
        {
            { MouseKeysEnum.Left, MouseButton.LeftButton },
            { MouseKeysEnum.Right, MouseButton.RightButton },
            { MouseKeysEnum.Middle, MouseButton.MiddleButton },
        };

        public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click)
        {
            switch (mode)
            {
                case KeyPressMode.Click:
                    _inputSim.Keyboard.KeyPress(_keyboardKeys[key]);
                    break;
                case KeyPressMode.Up:
                    _inputSim.Keyboard.KeyUp(_keyboardKeys[key]);
                    break;
                case KeyPressMode.Down:
                    _inputSim.Keyboard.KeyDown(_keyboardKeys[key]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public void MouseKeyPress(MouseKeysEnum key = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click)
        {
            switch (mode)
            {
                case KeyPressMode.Click:
                    _inputSim.Mouse.MouseButtonClick(_mouseKeys[key]);
                    break;
                case KeyPressMode.Down:
                    _inputSim.Mouse.MouseButtonDown(_mouseKeys[key]);
                    break;
                case KeyPressMode.Up:
                    _inputSim.Mouse.MouseButtonUp(_mouseKeys[key]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }


        public void MouseMove(MyPoint coords)
        {
            _inputSim.Mouse.MoveMouseBy(coords.X, coords.Y);
        }

        public void MouseWheel(bool up)
        {
            _inputSim.Mouse.VerticalScroll(up ? 3 : -3);
        }

        public void TextInput(string text)
        {
            _inputSim.Keyboard.TextEntry(text);
        }
    }
}
