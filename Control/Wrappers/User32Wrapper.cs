using Shared.Enums;
using Shared.Interfaces.Control;
using System.Runtime.InteropServices;

namespace Control.Wrappers
{
    public class User32Wrapper: IDisplayControl, IKeyboardControl, IMouseControl
    {
        #region Enums
        private enum MonitorState
        {
            MonitorStateOn = -1,
            MonitorStateOff = 2,
            MonitorStateStandBy = 1
        }

        private enum InputType : uint
        {
            Mouse = 0,
            Keyboard = 1
        }

        [Flags]
        private enum KeyboardFlag : uint
        {
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004
        }

        [Flags]
        private enum MouseFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            VerticalWheel = 0x0800
        }
        #endregion

        #region Structs
        private record struct MouseFlags(MouseFlag Up, MouseFlag Down);

        private struct Input
        {
            public uint Type;
            public UniversalInput Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct UniversalInput
        {
            [FieldOffset(0)]
            public Mouseinput Mouse;

            [FieldOffset(0)]
            public KeyboardInput Keyboard;
        }

        private struct KeyboardInput
        {
            public ushort KeyCode;
            public ushort Scan;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        private struct Mouseinput
        {
            public int X;
            public int Y;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }
        #endregion

        #region user32

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numberOfInputs, Input[] inputs, int sizeOfInputStructure);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);
        #endregion

        #region private

        private const int MouseWheelClickSize = 120;
        private readonly List<Input> _inputList = new();

        private readonly Dictionary<MouseKeysEnum, MouseFlags> _mouseFlags = new()
        {
            {
                MouseKeysEnum.Left, new MouseFlags(MouseFlag.LeftUp, MouseFlag.LeftDown)
            },
            {
                MouseKeysEnum.Right, new MouseFlags(MouseFlag.RightUp, MouseFlag.RightDown)
            },
            {
                MouseKeysEnum.Middle, new MouseFlags(MouseFlag.MiddleUp, MouseFlag.MiddleDown)
            }
        };

        private static void SetMonitorInState(MonitorState state) => SendMessage(0xFFFF, 0x112, 0xF170, (int)state);

        private void DispatchInput()
        {
            if (_inputList.Count == 0) return;

            SendInput((uint)_inputList.Count, _inputList.ToArray(), Marshal.SizeOf(typeof(Input)));

            _inputList.Clear();
        }

        private void KeyboardKeyPress(KeysEnum keyCode, KeyPressMode mode)
        {
            switch (mode)
            {
                case KeyPressMode.Up:
                    AddKeyUp(keyCode);
                    break;
                case KeyPressMode.Down:
                    AddKeyDown(keyCode);
                    break;
                case KeyPressMode.Click:
                    AddKeyPress(keyCode);
                    break;
                default:
                    return;
            }

            DispatchInput();
        }

        private void TextEntry(string text)
        {
            AddCharacters(text);
            DispatchInput();
        }

        private void MoveMouseBy(int pixelDeltaX, int pixelDeltaY)
        {
            AddRelativeMouseMovement(pixelDeltaX, pixelDeltaY);
            DispatchInput();
        }

        private void MouseButtonPress(MouseKeysEnum button, KeyPressMode mode)
        {
            switch (mode)
            {
                case KeyPressMode.Up:
                    AddMouseButtonUp(button);
                    break;
                case KeyPressMode.Down:
                    AddMouseButtonDown(button);
                    break;
                case KeyPressMode.Click:
                    AddMouseButtonClick(button);
                    break;
                default:
                    return;
            }

            DispatchInput();
        }

        private void VerticalScroll(int scrollAmountInClicks)
        {
            AddMouseVerticalWheelScroll(scrollAmountInClicks * MouseWheelClickSize);
            DispatchInput();
        }

        private static bool IsExtendedKey(KeysEnum keyCode)
        {
            return keyCode is
                KeysEnum.Alt or
                KeysEnum.RAlt or
                KeysEnum.Ctrl or
                KeysEnum.RControl or
                KeysEnum.Insert or
                KeysEnum.Del or
                KeysEnum.Home or
                KeysEnum.End or
                KeysEnum.PageUp or
                KeysEnum.PageDown or
                KeysEnum.ArrowRight or
                KeysEnum.ArrowUp or
                KeysEnum.ArrowLeft or
                KeysEnum.ArrowDown or
                KeysEnum.NumLock or
                KeysEnum.PrintScreen or
                KeysEnum.Divide;
        }

        private void AddKeyDown(KeysEnum keyCode)
        {
            var down = new Input
            {
                Type = (uint)InputType.Keyboard,
                Data =
                {
                    Keyboard = 
                    {
                        KeyCode = (ushort) keyCode,
                        Scan = (ushort)(MapVirtualKey((uint)keyCode, 0) & 0xFFU),
                        Flags = IsExtendedKey(keyCode) ? (uint) KeyboardFlag.ExtendedKey : 0,
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    }
                }
            };

            _inputList.Add(down);
        }

        private void AddKeyUp(KeysEnum keyCode)
        {
            var up = new Input
            {
                Type = (uint)InputType.Keyboard,
                Data =
                {
                    Keyboard =
                    {
                        KeyCode = (ushort) keyCode,
                        Scan = (ushort)(MapVirtualKey((uint)keyCode, 0) & 0xFFU),
                        Flags = (uint) (IsExtendedKey(keyCode)
                            ? KeyboardFlag.KeyUp | KeyboardFlag.ExtendedKey
                            : KeyboardFlag.KeyUp),
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    }
                }
            };

            _inputList.Add(up);
        }

        private void AddKeyPress(KeysEnum keyCode)
        {
            AddKeyDown(keyCode);
            AddKeyUp(keyCode);
        }

        private void AddCharacter(char character)
        {
            ushort scanCode = character;

            var down = new Input
            {
                Type = (uint)InputType.Keyboard,
                Data =
                {
                    Keyboard =
                    {
                        KeyCode = 0,
                        Scan = scanCode,
                        Flags = (uint)KeyboardFlag.Unicode,
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    }
                }
            };

            var up = new Input
            {
                Type = (uint)InputType.Keyboard,
                Data =
                {
                    Keyboard =
                    {
                        KeyCode = 0,
                        Scan = scanCode,
                        Flags = (uint)(KeyboardFlag.KeyUp | KeyboardFlag.Unicode),
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    }
                }
            };

            // Handle extended keys:
            // If the scan code is preceded by a prefix byte that has the value 0xE0 (224),
            // we need to include the KEYEVENTF_EXTENDEDKEY flag in the Flags property. 
            if ((scanCode & 0xFF00) == 0xE000)
            {
                down.Data.Keyboard.Flags |= (uint)KeyboardFlag.ExtendedKey;
                up.Data.Keyboard.Flags |= (uint)KeyboardFlag.ExtendedKey;
            }

            _inputList.Add(down);
            _inputList.Add(up);
        }

        private void AddCharacters(IEnumerable<char> characters)
        {
            foreach (var character in characters)
            {
                AddCharacter(character);
            }
        }

        private void AddCharacters(string characters)
        {
            AddCharacters(characters.ToCharArray());
        }

        private void AddRelativeMouseMovement(int x, int y)
        {
            var movement = new Input
            {
                Type = (uint)InputType.Mouse,
                Data =
                {
                    Mouse =
                    {
                        Flags = (uint)MouseFlag.Move,
                        X = x,
                        Y = y
                    }
                }
            };

            _inputList.Add(movement);
        }

        private void AddMouseButtonDown(MouseKeysEnum button)
        {
            var buttonDown = new Input
            {
                Type = (uint)InputType.Mouse,
                Data =
                {
                    Mouse =
                    {
                        Flags = (uint)_mouseFlags[button].Down
                    }
                }
            };

            _inputList.Add(buttonDown);
        }

        private void AddMouseButtonUp(MouseKeysEnum button)
        {
            var buttonUp = new Input
            {
                Type = (uint)InputType.Mouse,
                Data =
                {
                    Mouse =
                    {
                        Flags = (uint)_mouseFlags[button].Up
                    }
                }
            };

            _inputList.Add(buttonUp);
        }

        private void AddMouseButtonClick(MouseKeysEnum button)
        {
            AddMouseButtonDown(button);
            AddMouseButtonUp(button);
        }

        private void AddMouseVerticalWheelScroll(int scrollAmount)
        {
            var scroll = new Input
            {
                Type = (uint)InputType.Mouse,
                Data =
                {
                    Mouse =
                    {
                        Flags = (uint)MouseFlag.VerticalWheel,
                        MouseData = (uint)scrollAmount
                    }
                }
            };

            _inputList.Add(scroll);
        }
        #endregion

        public void Darken() => SetMonitorInState(MonitorState.MonitorStateOff);

        public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click) => KeyboardKeyPress(key, mode);

        public void TextInput(string text) => TextEntry(text);

        public void Move(int x, int y) => MoveMouseBy(x, y);

        public void ButtonPress(MouseKeysEnum button = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click) =>
            MouseButtonPress(button, mode);

        public void Wheel(bool up) => VerticalScroll(up ? -1 : 1);



        
            
           
            
            

           

           

            
            
            

            

            

            

            
    }
}
