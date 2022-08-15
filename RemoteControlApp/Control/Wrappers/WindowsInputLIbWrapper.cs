using Shared.Enums;
using Shared.Interfaces.Control;
using WindowsInputLib;
using WindowsInputLib.Native;

namespace RemoteControlApp.Control.Wrappers
{
    public class WindowsInputLibWrapper : IKeyboardControl, IMouseControl
    {
        private readonly InputSimulator _inputSim = new();
        private static readonly Dictionary<KeysEnum, VirtualKeyCode> KeyboardKeys = new()
        {
            { KeysEnum.Enter, VirtualKeyCode.Enter },
            { KeysEnum.ArrowRight, VirtualKeyCode.Right },
            { KeysEnum.ArrowLeft, VirtualKeyCode.Left },
            { KeysEnum.MediaForth, VirtualKeyCode.MediaNextTrack },
            { KeysEnum.MediaBack, VirtualKeyCode.MediaPrevTrack },
            { KeysEnum.MediaPlayPause, VirtualKeyCode.MediaPlayPause },
        };
        private static readonly Dictionary<MouseKeysEnum, MouseButton> MouseKeys = new()
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
                    _inputSim.Keyboard.KeyPress(KeyboardKeys[key]);
                    break;
                case KeyPressMode.Up:
                    _inputSim.Keyboard.KeyUp(KeyboardKeys[key]);
                    break;
                case KeyPressMode.Down:
                    _inputSim.Keyboard.KeyDown(KeyboardKeys[key]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public void KeyPress(MouseKeysEnum key = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click)
        {
            switch (mode)
            {
                case KeyPressMode.Click:
                    _inputSim.Mouse.MouseButtonClick(MouseKeys[key]);
                    break;
                case KeyPressMode.Down:
                    _inputSim.Mouse.MouseButtonDown(MouseKeys[key]);
                    break;
                case KeyPressMode.Up:
                    _inputSim.Mouse.MouseButtonUp(MouseKeys[key]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public void Move(int x, int y)
        {
            _inputSim.Mouse.MoveMouseBy(x, y);
        }

        public void Wheel(bool up)
        {
            _inputSim.Mouse.VerticalScroll(up ? 1 : -1);
        }

        public void TextInput(string text)
        {
            _inputSim.Keyboard.TextEntry(text);
        }
    }
}
