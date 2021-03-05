    using System.Collections.Generic;
using WindowsInputLib;
using WindowsInputLib.Native;
using RemoteControl.Core.Enums;
using RemoteControl.Core.Interfaces;

namespace RemoteControl.Core.Providers
{
    internal class MyInputProvider : IInputProvider
    {
        public IKeyboard Keyboard { get; }
        public IMouse Mouse { get; }

        public MyInputProvider()
        {
            var inputSim = new InputSimulator();

            Keyboard = new MyKeyboard(inputSim);
            Mouse = new MyMouse(inputSim);
        }

        private class MyKeyboard : IKeyboard
        {
            private readonly InputSimulator _inputSim;

            private readonly Dictionary<KeysEnum, VirtualKeyCode> _keyboardKeys = new Dictionary<KeysEnum, VirtualKeyCode>()
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
                },
                {
                    KeysEnum.MediaBack,
                    VirtualKeyCode.MediaPrevTrack
                },
                {
                    KeysEnum.MediaForth,
                    VirtualKeyCode.MediaNextTrack
                }
            };

            public MyKeyboard(InputSimulator sim)
            {
                _inputSim = sim;
            }

            public void KeyDown(KeysEnum key)
            {
                _inputSim.Keyboard.KeyDown(_keyboardKeys[key]);
            }

            public void KeyPress(KeysEnum key)
            {
                _inputSim.Keyboard.KeyPress(_keyboardKeys[key]);
            }

            public void KeyUp(KeysEnum key)
            {
                _inputSim.Keyboard.KeyUp(_keyboardKeys[key]);
            }

            public void TextEntry(string text)
            {
                _inputSim.Keyboard.TextEntry(text);
            }
        }

        private class MyMouse : IMouse
        {
            private readonly InputSimulator _inputSim;


            private readonly Dictionary<MouseKeysEnum, MouseButton> _mouseKeys = new Dictionary<MouseKeysEnum, MouseButton>()
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
                _inputSim.Mouse.MouseButtonClick(_mouseKeys[key]);
            }

            public void MouseButtonDown(MouseKeysEnum key)
            {
                _inputSim.Mouse.MouseButtonDown(_mouseKeys[key]);
            }

            public void MouseButtonUp(MouseKeysEnum key)
            {
                _inputSim.Mouse.MouseButtonUp(_mouseKeys[key]);
            }

            public void MoveMouseBy(int x, int y)
            {
                _inputSim.Mouse.MoveMouseBy(x, y);
            }

            public void VerticalScroll(int y)
            {
                _inputSim.Mouse.VerticalScroll(y);
            }
        }
    }
}
