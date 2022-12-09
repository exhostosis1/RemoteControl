using System.Runtime.InteropServices;
using ControlProviders.Abstract;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace ControlProviders;

public class User32Provider : BaseProvider, IDisplayControlProvider, IKeyboardControlProvider, IMouseControlProvider
{
    public User32Provider(ILogger logger) : base(logger)
    {
    }

    #region Enums
    private enum User32KeyCodes
    {
        Backspace = 0x08,
        Tab = 0x09,
        Enter = 0x0D,
        Shift = 0x10,
        Ctrl = 0x11,
        Alt = 0x12,
        Pause = 0x13,
        CapsLock = 0x14,
        Escape = 0x1B,
        Space = 0x20,
        PageUp = 0x21,
        PageDown = 0x22,
        End = 0x23,
        Home = 0x24,
        ArrowLeft = 0x25,
        ArrowUp = 0x26,
        ArrowRight = 0x27,
        ArrowDown = 0x28,
        Select = 0x29,
        Print = 0x2A,
        PrintScreen = 0x2C,
        Insert = 0x2D,
        Del = 0x2E,
        Help = 0x2F,
        Key0 = 0x30,
        Key1 = 0x31,
        Key2 = 0x32,
        Key3 = 0x33,
        Key4 = 0x34,
        Key5 = 0x35,
        Key6 = 0x36,
        Key7 = 0x37,
        Key8 = 0x38,
        Key9 = 0x39,
        KeyA = 0x41,
        KeyB = 0x42,
        KeyC = 0x43,
        KeyD = 0x44,
        KeyE = 0x45,
        KeyF = 0x46,
        KeyG = 0x47,
        KeyH = 0x48,
        KeyI = 0x49,
        KeyJ = 0x4A,
        KeyK = 0x4B,
        KeyL = 0x4C,
        KeyM = 0x4D,
        KeyN = 0x4E,
        KeyO = 0x4F,
        KeyP = 0x50,
        KeyQ = 0x51,
        KeyR = 0x52,
        KeyS = 0x53,
        KeyT = 0x54,
        KeyU = 0x55,
        KeyV = 0x56,
        KeyW = 0x57,
        KeyX = 0x58,
        KeyY = 0x59,
        KeyZ = 0x5A,
        Win = 0x5B,
        Sleep = 0x5F,
        Numpad0 = 0x60,
        Numpad1 = 0x61,
        Numpad2 = 0x62,
        Numpad3 = 0x63,
        Numpad4 = 0x64,
        Numpad5 = 0x65,
        Numpad6 = 0x66,
        Numpad7 = 0x67,
        Numpad8 = 0x68,
        Numpad9 = 0x69,
        Multiply = 0x6A,
        Add = 0x6B,
        Separator = 0x6C,
        Substract = 0x6D,
        Decimal = 0x6E,
        Divide = 0x6F,
        F1 = 0x70,
        F2 = 0x71,
        F3 = 0x72,
        F4 = 0x73,
        F5 = 0x74,
        F6 = 0x75,
        F7 = 0x76,
        F8 = 0x77,
        F9 = 0x78,
        F10 = 0x79,
        F11 = 0x7A,
        F12 = 0x7B,
        NumLock = 0x90,
        ScrollLock = 0x91,
        LShift = 0xA0,
        RShift = 0xA1,
        LControl = 0xA2,
        RControl = 0xA3,
        LAlt = 0xA4,
        RAlt = 0xA5,
        BrowserBack = 0xA6,
        BrowserForward = 0xA7,
        BrowserRefresh = 0xA8,
        BrowserStop = 0xA9,
        BrowserSearch = 0xAA,
        BrowserFavorites = 0xAB,
        BrowserHome = 0xAC,
        VolumeMute = 0xAD,
        VolumeDown = 0xAE,
        VolumeUp = 0xAF,
        MediaNext = 0xB0,
        MediaPrev = 0xB1,
        MediaStop = 0xB2,
        MediaPlayPause = 0xB3,
        Mail = 0xB4,
        MediaSelect = 0xB5,
    }
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
        KeyDown = 0x0000,
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
#pragma warning disable CS0649
    private struct KeyboardInput
    {
        public ushort KeyCode;
        public ushort Scan;
        public uint Flags;
        public uint Time;
        public nint ExtraInfo;
    }

    private struct Mouseinput
    {
        public int X;
        public int Y;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public nint ExtraInfo;
    }
#pragma warning restore CS0649
    #endregion

    #region user32

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint numberOfInputs, Input[] inputs, int sizeOfInputStructure);

    [DllImport("user32.dll")]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);

    [DllImport("user32.dll")]
    private static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);
    #endregion

    #region Dictionaries

    private static readonly Dictionary<KeysEnum, User32KeyCodes> KeysToKeyCodes = new()
    {
        {
            KeysEnum.ArrowLeft, User32KeyCodes.ArrowLeft
        },
        {
            KeysEnum.ArrowRight, User32KeyCodes.ArrowRight
        },
        {
            KeysEnum.MediaNext, User32KeyCodes.MediaNext
        },
        {
            KeysEnum.MediaPrev, User32KeyCodes.MediaPrev
        },
        {
            KeysEnum.MediaPlayPause, User32KeyCodes.MediaPlayPause
        },
        {
            KeysEnum.Enter, User32KeyCodes.Enter
        },
        {
            KeysEnum.VolumeUp, User32KeyCodes.VolumeUp
        },
        {
            KeysEnum.VolumeDown, User32KeyCodes.VolumeDown
        },
        {
            KeysEnum.Mute, User32KeyCodes.VolumeMute
        }
    };

    private static readonly Dictionary<MouseButtons, MouseFlags> MouseButtonsToFlags = new()
    {
        {
            MouseButtons.Left, new MouseFlags(MouseFlag.LeftUp, MouseFlag.LeftDown)
        },
        {
            MouseButtons.Right, new MouseFlags(MouseFlag.RightUp, MouseFlag.RightDown)
        },
        {
            MouseButtons.Middle, new MouseFlags(MouseFlag.MiddleUp, MouseFlag.MiddleDown)
        }
    };

    #endregion

    #region private

    private const uint Length = 1;
    private const int MouseWheelClickSize = 120;
    private readonly Input[] _buffer = new Input[Length];
    private readonly int _size = Marshal.SizeOf(typeof(Input));

    private static void SetMonitorInState(MonitorState state) => SendMessage(0xFFFF, 0x112, 0xF170, (int)state);

    private void DispatchInput() => SendInput(Length, _buffer, _size);

    private static bool IsExtendedKey(User32KeyCodes keyCode)
    {
        return keyCode is
            User32KeyCodes.Alt or
            User32KeyCodes.RAlt or
            User32KeyCodes.Ctrl or
            User32KeyCodes.RControl or
            User32KeyCodes.Insert or
            User32KeyCodes.Del or
            User32KeyCodes.Home or
            User32KeyCodes.End or
            User32KeyCodes.PageUp or
            User32KeyCodes.PageDown or
            User32KeyCodes.ArrowRight or
            User32KeyCodes.ArrowUp or
            User32KeyCodes.ArrowLeft or
            User32KeyCodes.ArrowDown or
            User32KeyCodes.NumLock or
            User32KeyCodes.PrintScreen or
            User32KeyCodes.Divide;
    }

    private void SendKeyInput(User32KeyCodes keyCode, bool up)
    {
        _buffer[0].Type = (uint)InputType.Keyboard;
        var flag = up ? KeyboardFlag.KeyUp : KeyboardFlag.KeyDown;

        _buffer[0].Data.Keyboard = new KeyboardInput
        {
            KeyCode = (ushort)keyCode,
            Scan = (ushort)(MapVirtualKey((uint)keyCode, 0) & 0xFFU),
            Flags = (uint)(IsExtendedKey(keyCode) ? KeyboardFlag.ExtendedKey | flag : flag),
        };

        DispatchInput();
    }

    private void SendCharInput(char character, bool up)
    {
        _buffer[0].Type = (uint)InputType.Keyboard;

        _buffer[0].Data.Keyboard = new KeyboardInput
        {
            Scan = character,
            Flags = (uint)(KeyboardFlag.Unicode | (up ? KeyboardFlag.KeyUp : KeyboardFlag.KeyDown)),
        };

        if ((character & 0xFF00) == 0xE000)
            _buffer[0].Data.Keyboard.Flags |= (uint)KeyboardFlag.ExtendedKey;

        DispatchInput();
    }

    private void SendMouseInput(int x, int y)
    {
        _buffer[0].Type = (uint)InputType.Mouse;

        _buffer[0].Data.Mouse = new Mouseinput
        {
            Flags = (uint)MouseFlag.Move,
            X = x,
            Y = y,
        };

        DispatchInput();
    }

    private void SendMouseInput(MouseFlag button)
    {
        _buffer[0].Type = (uint)InputType.Mouse;

        _buffer[0].Data.Mouse = new Mouseinput
        {
            Flags = (uint)button,
        };

        DispatchInput();
    }

    private void SendScrollInput(int scrollAmount)
    {
        _buffer[0].Type = (uint)InputType.Mouse;

        _buffer[0].Data.Mouse = new Mouseinput
        {
            Flags = (uint)MouseFlag.VerticalWheel,
            MouseData = (uint)scrollAmount,
        };

        DispatchInput();
    }
    #endregion

    public void Darken() => SetMonitorInState(MonitorState.MonitorStateOff);

    public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click)
    {
        if (mode.HasFlag(KeyPressMode.Down))
            SendKeyInput(KeysToKeyCodes[key], false);

        if (mode.HasFlag(KeyPressMode.Up))
            SendKeyInput(KeysToKeyCodes[key], true);
    }

    public void TextInput(string text)
    {
        foreach (var c in text.ToCharArray())
        {
            SendCharInput(c, false);
            SendCharInput(c, true);
        }
    }

    public void Move(int x, int y) => SendMouseInput(x, y);

    public void ButtonPress(MouseButtons button = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click)
    {
        if(mode.HasFlag(KeyPressMode.Down))
            SendMouseInput(MouseButtonsToFlags[button].Down);

        if (mode.HasFlag(KeyPressMode.Up))
            SendMouseInput(MouseButtonsToFlags[button].Up);
    }

    public void Wheel(bool up) => SendScrollInput(up ? MouseWheelClickSize : -MouseWheelClickSize);
}