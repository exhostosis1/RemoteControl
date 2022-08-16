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
        private readonly Input[] _buffer = new Input[Length];
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

        private void DispatchInput() => SendInput(Length, _buffer, _size);

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

        

        private void SendInput(KeysEnum keyCode, bool up)
        {
            var flag = up ? KeyboardFlag.KeyUp : 0;

            _buffer[0].Type = (uint)InputType.Keyboard;

            _buffer[0].Data.Keyboard = new KeyboardInput
            {
                KeyCode = (ushort) keyCode,
                Scan = (ushort) (MapVirtualKey((uint) keyCode, 0) & 0xFFU),
                Flags = (uint) (IsExtendedKey(keyCode) ? flag | KeyboardFlag.ExtendedKey : flag),
            };

            DispatchInput();
        }

        private void SendInput(KeysEnum key)
        {
            SendInput(key, false);
            SendInput(key, true);
        }

        private void SendInput(char character, bool up)
        {
            _buffer[0].Type = (uint) InputType.Keyboard;

            _buffer[0].Data.Keyboard = new KeyboardInput
            {
                Scan = character,
                Flags = (uint) (KeyboardFlag.Unicode | (up ? KeyboardFlag.KeyUp : 0)),
            };

            if ((character & 0xFF00) == 0xE000)
                _buffer[0].Data.Keyboard.Flags |= (uint)KeyboardFlag.ExtendedKey;

            DispatchInput();
        }

        private void SendInput(char character)
        {
            SendInput(character, false);
            SendInput(character, true);
        }

        private void SendInput(int x, int y)
        {
            _buffer[0].Type = (uint) InputType.Mouse;

            _buffer[0].Data.Mouse = new Mouseinput
            {
                Flags = (uint)MouseFlag.Move,
                X = x,
                Y = y,
            };

            DispatchInput();
        }

        private void SendInput(MouseKeysEnum button, bool up)
        {
            _buffer[0].Type = (uint) InputType.Mouse;

            _buffer[0].Data.Mouse = new Mouseinput
            {
                Flags = (uint) (up ? KeysToFlags[button].Up : KeysToFlags[button].Down),
            };

            DispatchInput();
        }

        private void SendInput(MouseKeysEnum button)
        {
            SendInput(button, false);
            SendInput(button, true);
        }

        private void SendInput(int scrollAmount)
        {
            _buffer[0].Type = (uint)InputType.Mouse;

            _buffer[0].Data.Mouse = new Mouseinput
            {
                Flags = (uint) MouseFlag.VerticalWheel,
                MouseData = (uint) scrollAmount,
            };

            DispatchInput();
        }
        #endregion

        public void Darken() => SetMonitorInState(MonitorState.MonitorStateOff);

        public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click)
        {
            if (mode == KeyPressMode.Click)
                SendInput(key);
            else
                SendInput(key, mode == KeyPressMode.Up);
        }

        public void TextInput(string text)
        {
            foreach (var c in text.ToCharArray())
            {
                SendInput(c);
            }
        }

        public void Move(int x, int y) => SendInput(x, y);

        public void ButtonPress(MouseKeysEnum button = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click)
        {
            if (mode == KeyPressMode.Click)
                SendInput(button);
            else
                SendInput(button, mode == KeyPressMode.Up);
        }

        public void Wheel(bool up) => SendInput(up ? MouseWheelClickSize : -MouseWheelClickSize);
    }
}