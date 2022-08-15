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

        private const uint Length = 1;
        private const int MouseWheelClickSize = 120;
        private readonly Input[] _inputList = new Input[Length];
        private readonly int _size = Marshal.SizeOf(typeof(Input));

        private static readonly Dictionary<MouseKeysEnum, MouseFlags> KeysToFlags = new()
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

        private void DispatchInput() => SendInput(Length, _inputList, _size);

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

        private void KeyboardKeyPress(KeysEnum keyCode, KeyPressMode mode)
        {
            if (mode == KeyPressMode.Click)
            {
                SendInput(keyCode, KeyPressMode.Down);
                SendInput(keyCode, KeyPressMode.Up);
            }
            else
            {
                SendInput(keyCode, mode);
            }
        }

        private void TextEntry(string text)
        {
            foreach (var c in text.ToCharArray())
            {
                SendInput(c, KeyPressMode.Down);
                SendInput(c, KeyPressMode.Up);
            }
        }

        private void MoveMouseBy(int pixelDeltaX, int pixelDeltaY)
        {
            SendInput(pixelDeltaX, pixelDeltaY);
        }

        private void MouseButtonPress(MouseKeysEnum button, KeyPressMode mode)
        {
            if (mode == KeyPressMode.Click)
            {
                SendInput(button, KeyPressMode.Down);
                SendInput(button, KeyPressMode.Up);
            }
            else
            {
                SendInput(button, mode);
            }
        }

        private void VerticalScroll(int scrollAmountInClicks)
        {
            SendInput(scrollAmountInClicks * MouseWheelClickSize);
        }

        private void SendInput(KeysEnum keyCode, KeyPressMode mode)
        {
            var flag = mode == KeyPressMode.Up ? KeyboardFlag.KeyUp : 0;

            _inputList[0].Type = (uint)InputType.Keyboard;

            _inputList[0].Data.Keyboard = new KeyboardInput
            {
                KeyCode = (ushort) keyCode,
                Scan = (ushort) (MapVirtualKey((uint) keyCode, 0) & 0xFFU),
                Flags = (uint) (IsExtendedKey(keyCode) ? flag | KeyboardFlag.ExtendedKey : flag),
            };

            DispatchInput();
        }

        private void SendInput(char character, KeyPressMode mode)
        {
            var flag = mode == KeyPressMode.Up ? KeyboardFlag.KeyUp : 0;

            _inputList[0].Type = (uint) InputType.Keyboard;

            _inputList[0].Data.Keyboard = new KeyboardInput
            {
                Scan = character,
                Flags = (uint) (KeyboardFlag.Unicode | flag),
            };

            if ((character & 0xFF00) == 0xE000)
                _inputList[0].Data.Keyboard.Flags |= (uint)KeyboardFlag.ExtendedKey;

            DispatchInput();
        }

        private void SendInput(int x, int y)
        {
            _inputList[0].Type = (uint) InputType.Mouse;

            _inputList[0].Data.Mouse = new Mouseinput
            {
                Flags = (uint)MouseFlag.Move,
                X = x,
                Y = y,
            };

            DispatchInput();
        }

        private void SendInput(MouseKeysEnum button, KeyPressMode mode)
        {
            _inputList[0].Type = (uint) InputType.Mouse;

            _inputList[0].Data.Mouse = new Mouseinput
            {
                Flags = (uint) (mode == KeyPressMode.Up ? KeysToFlags[button].Up : KeysToFlags[button].Down),
            };

            DispatchInput();
        }

        private void SendInput(int scrollAmount)
        {
            _inputList[0].Type = (uint)InputType.Mouse;

            _inputList[0].Data.Mouse = new Mouseinput
            {
                Flags = (uint) MouseFlag.VerticalWheel,
                MouseData = (uint) scrollAmount,
            };

            DispatchInput();
        }
        #endregion

        public void Darken() => SetMonitorInState(MonitorState.MonitorStateOff);

        public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click) => KeyboardKeyPress(key, mode);

        public void TextInput(string text) => TextEntry(text);

        public void Move(int x, int y) => MoveMouseBy(x, y);

        public void ButtonPress(MouseKeysEnum button = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click) =>
            MouseButtonPress(button, mode);

        public void Wheel(bool up) => VerticalScroll(up ? 1 : -1);
    }
}