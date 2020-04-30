using RemoteControlCore.Enums;
using RemoteControlCore.Interfaces;
using System.Collections.Generic;
using WindowsInputLib;
using WindowsInputLib.Native;

namespace RemoteControlCore.Providers
{
    internal class MyInputProvider : IInputProvider
    {
        public IKeyboard Keyboard { get; set; }
        public IMouse Mouse { get; set; }

        private InputSimulator _inputSim;

        public MyInputProvider()
        {
            _inputSim = new InputSimulator();

            Keyboard = new MyKeyboard(_inputSim);
            Mouse = new MyMouse(_inputSim);
        }

        internal class MyKeyboard : IKeyboard
        {
            InputSimulator _inputSim;

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

            public MyKeyboard(InputSimulator sim)
            {
                _inputSim = sim;
            }

            public void KeyDown(KeysEnum key)
            {
                _inputSim.Keyboard.KeyDown(keyboardKeys[key]);
            }

            public void KeyPress(KeysEnum key)
            {
                _inputSim.Keyboard.KeyPress(keyboardKeys[key]);
            }

            public void KeyUp(KeysEnum key)
            {
                _inputSim.Keyboard.KeyUp(keyboardKeys[key]);
            }

            public void TextEntry(string text)
            {
                _inputSim.Keyboard.TextEntry(text);
            }
        }

        internal class MyMouse : IMouse
        {
            InputSimulator _inputSim;


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

            public MyMouse(InputSimulator sim)
            {
                _inputSim = sim;
            }

            public void MouseButtonClick(MouseKeysEnum key)
            {
                _inputSim.Mouse.MouseButtonClick(mouseKeys[key]);
            }

            public void MouseButtonDown(MouseKeysEnum key)
            {
                _inputSim.Mouse.MouseButtonDown(mouseKeys[key]);
            }

            public void MouseButtonUp(MouseKeysEnum key)
            {
                _inputSim.Mouse.MouseButtonUp(mouseKeys[key]);
            }

            public void MoveMouseBy(int X, int Y)
            {
                _inputSim.Mouse.MoveMouseBy(X, Y);
            }

            public void VerticalScroll(int Y)
            {
                _inputSim.Mouse.VerticalScroll(Y);
            }
        }
    }
}
