using System.Collections;
using System.Runtime.InteropServices;
using Shared.Enums;
using Shared.Interfaces.Control;

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

        private static void SetMonitorInState(MonitorState state) => SendMessage(0xFFFF, 0x112, 0xF170, (int)state);

        private void DispatchInput(Input[] inputs)
        {
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));
            if (inputs.Length == 0) throw new ArgumentException("The input array was empty", nameof(inputs));

            var successful = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));

            if (successful != inputs.Length)
                throw new Exception(
                    "Some simulated input commands were not sent successfully. The most common reason for this happening are the security features of Windows including User Interface Privacy Isolation (UIPI). Your application can only send commands to applications of the same or lower elevation. Similarly certain commands are restricted to Accessibility/UIAutomation applications. Refer to the project home page and the code samples for more information.");
        }

        private void KeyboardKeyPress(KeysEnum keyCode, KeyPressMode mode)
        {
            var inputList = mode switch
            {
                KeyPressMode.Up => new InputBuilder().AddKeyUp(keyCode).ToArray(),
                KeyPressMode.Down => new InputBuilder().AddKeyDown(keyCode).ToArray(),
                KeyPressMode.Click => new InputBuilder().AddKeyPress(keyCode).ToArray(),
                _ => Array.Empty<Input>()
            };

            DispatchInput(inputList);
        }

        private void TextEntry(string text)
        {
            var inputList = new InputBuilder().AddCharacters(text).ToArray();
            DispatchInput(inputList);
        }

        private void MoveMouseBy(int pixelDeltaX, int pixelDeltaY)
        {
            var inputList = new InputBuilder().AddRelativeMouseMovement(pixelDeltaX, pixelDeltaY).ToArray();
            DispatchInput(inputList);
        }

        private void MouseButtonPress(MouseKeysEnum button, KeyPressMode mode)
        {
            var inputList = mode switch
            {
                KeyPressMode.Up => new InputBuilder().AddMouseButtonUp(button).ToArray(),
                KeyPressMode.Down => new InputBuilder().AddMouseButtonDown(button).ToArray(),
                KeyPressMode.Click => new InputBuilder().AddMouseButtonClick(button).ToArray(),
                _ => Array.Empty<Input>()
            };

            DispatchInput(inputList);
        }

        private void VerticalScroll(int scrollAmountInClicks)
        {
            var inputList = new InputBuilder().AddMouseVerticalWheelScroll(scrollAmountInClicks * MouseWheelClickSize).ToArray();
            DispatchInput(inputList);
        }
        #endregion

        public void Darken() => SetMonitorInState(MonitorState.MonitorStateOff);

        public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click) => KeyboardKeyPress(key, mode);

        public void TextInput(string text) => TextEntry(text);

        public void Move(int x, int y) => MoveMouseBy(x, y);

        public void ButtonPress(MouseKeysEnum button = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click) =>
            MouseButtonPress(button, mode);

        public void Wheel(bool up) => VerticalScroll(up ? -1 : 1);

        private class InputBuilder : IEnumerable<Input>
        {
            private readonly List<Input> _inputList = new();
            
            public Input[] ToArray()
            {
                return _inputList.ToArray();
            }

            public IEnumerator<Input> GetEnumerator()
            {
                return _inputList.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public Input this[int position] => _inputList[position];

            public static bool IsExtendedKey(KeysEnum keyCode)
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
            
            public InputBuilder AddKeyDown(KeysEnum keyCode)
            {
                var down =
                    new Input
                    {
                        Type = (uint)InputType.Keyboard,
                        Data =
                                {
                                Keyboard =
                                    new KeyboardInput
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
                return this;
            }
            
            public InputBuilder AddKeyUp(KeysEnum keyCode)
            {
                var up =
                    new Input
                    {
                        Type = (uint)InputType.Keyboard,
                        Data =
                                {
                                Keyboard =
                                    new KeyboardInput
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
                return this;
            }
            
            public InputBuilder AddKeyPress(KeysEnum keyCode)
            {
                AddKeyDown(keyCode);
                AddKeyUp(keyCode);
                return this;
            }

            public InputBuilder AddCharacter(char character)
            {
                ushort scanCode = character;

                var down = new Input
                {
                    Type = (uint)InputType.Keyboard,
                    Data =
                                   {
                                       Keyboard =
                                           new KeyboardInput
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
                                         new KeyboardInput
                                             {
                                                 KeyCode = 0,
                                                 Scan = scanCode,
                                                 Flags =
                                                     (uint)(KeyboardFlag.KeyUp | KeyboardFlag.Unicode),
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
                return this;
            }

            public InputBuilder AddCharacters(IEnumerable<char> characters)
            {
                foreach (var character in characters)
                {
                    AddCharacter(character);
                }
                return this;
            }

            public InputBuilder AddCharacters(string characters)
            {
                return AddCharacters(characters.ToCharArray());
            }

            public InputBuilder AddRelativeMouseMovement(int x, int y)
            {
                var movement = new Input { Type = (uint)InputType.Mouse };
                movement.Data.Mouse.Flags = (uint)MouseFlag.Move;
                movement.Data.Mouse.X = x;
                movement.Data.Mouse.Y = y;

                _inputList.Add(movement);

                return this;
            }
            
            public InputBuilder AddMouseButtonDown(MouseKeysEnum button)
            {
                var buttonDown = new Input { Type = (uint)InputType.Mouse };
                buttonDown.Data.Mouse.Flags = (uint)ToMouseButtonDownFlag(button);

                _inputList.Add(buttonDown);

                return this;
            }

            public InputBuilder AddMouseButtonUp(MouseKeysEnum button)
            {
                var buttonUp = new Input { Type = (uint)InputType.Mouse };
                buttonUp.Data.Mouse.Flags = (uint)ToMouseButtonUpFlag(button);
                _inputList.Add(buttonUp);

                return this;
            }

            public InputBuilder AddMouseButtonClick(MouseKeysEnum button)
            {
                return AddMouseButtonDown(button).AddMouseButtonUp(button);
            }

            public InputBuilder AddMouseVerticalWheelScroll(int scrollAmount)
            {
                var scroll = new Input { Type = (uint)InputType.Mouse };
                scroll.Data.Mouse.Flags = (uint)MouseFlag.VerticalWheel;
                scroll.Data.Mouse.MouseData = (uint)scrollAmount;

                _inputList.Add(scroll);

                return this;
            }

            private static MouseFlag ToMouseButtonDownFlag(MouseKeysEnum button)
            {
                return button switch
                {
                    MouseKeysEnum.Left => MouseFlag.LeftDown,
                    MouseKeysEnum.Middle => MouseFlag.MiddleDown,
                    MouseKeysEnum.Right => MouseFlag.RightDown,
                    _ => MouseFlag.LeftDown
                };
            }

            private static MouseFlag ToMouseButtonUpFlag(MouseKeysEnum button)
            {
                return button switch
                {
                    MouseKeysEnum.Left => MouseFlag.LeftUp,
                    MouseKeysEnum.Middle => MouseFlag.MiddleUp,
                    MouseKeysEnum.Right => MouseFlag.RightUp,
                    _ => MouseFlag.LeftUp
                };
            }
        }
    }
}
