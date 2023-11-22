using Shared.ControlProviders.Input;
using Shared.Enums;
using System.Runtime.InteropServices;

namespace ControlProviders.Wrappers;

public partial class User32Wrapper : IKeyboardInput, IDisplayInput, IMouseInput
{
    private const uint Length = 1;
    private readonly Input[] _buffer = new Input[Length];
    private readonly int _size = Marshal.SizeOf(typeof(Input));
    private const string User32LibraryName = "user32.dll";

    private record struct Input
    {
        public uint Type;
        public UniversalInput Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    private record struct UniversalInput
    {
        [FieldOffset(0)]
        public MouseInput Mouse;

        [FieldOffset(0)]
        public KeyboardInput Keyboard;
    }
#pragma warning disable CS0649
    private record struct KeyboardInput
    {
        public ushort KeyCode;
        public ushort Scan;
        public uint Flags;
        public uint Time;
        public nint ExtraInfo;
    }

    private record struct MouseInput
    {
        public int X;
        public int Y;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public nint ExtraInfo;
    }

    private record struct MouseFlags(MouseFlag Up, MouseFlag Down);

    [LibraryImport(User32LibraryName, SetLastError = true)]
    private static partial uint SendInput(uint numberOfInputs, Input[] inputs, int sizeOfInputStructure);

    [LibraryImport(User32LibraryName, EntryPoint = "MapVirtualKeyW")]
    private static partial uint MapVirtualKey(uint uCode, uint uMapType);

    [LibraryImport(User32LibraryName, EntryPoint = "SendMessageW")]
    private static partial int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

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
        },
        {
            KeysEnum.BrowserBack, User32KeyCodes.BrowserBack
        },
        {
            KeysEnum.BrowserForward, User32KeyCodes.BrowserForward
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

    public User32Wrapper()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");
    }

    private void DispatchInput()
    {
        SendInput(Length, _buffer, _size);
    }

    public void SetState(MonitorState state)
    {
        SendMessage(0xFFFF, 0x112, 0xF170, (int)state);
    }

    public void SendKey(KeysEnum key, KeyPressMode mode)
    {
        if (mode.HasFlag(KeyPressMode.Down))
            SendKey(key);
        if (mode.HasFlag(KeyPressMode.Up))
            SendKey(key, true);
    }

    public void SendKey(KeysEnum keyCode, bool up = false)
    {
        var user32KeyCode = KeysToKeyCodes[keyCode];

        _buffer[0].Type = (uint)InputType.Keyboard;
        var flag = up ? KeyboardFlag.KeyUp : KeyboardFlag.KeyDown;

        _buffer[0].Data.Keyboard = new KeyboardInput
        {
            KeyCode = (ushort)user32KeyCode,
            Scan = (ushort)(MapVirtualKey((uint)user32KeyCode, 0) & 0xFFU),
            Flags = (uint)(IsExtendedKey(user32KeyCode) ? KeyboardFlag.ExtendedKey | flag : flag),
        };

        DispatchInput();
    }

    public void SendCharInput(char character, bool up = false)
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

    public void SendText(string text)
    {
        foreach (var c in text)
        {
            SendCharInput(c);
            SendCharInput(c, true);
        }
    }

    public void SendMouseMove(int x, int y)
    {
        _buffer[0].Type = (uint)InputType.Mouse;

        _buffer[0].Data.Mouse = new MouseInput
        {
            Flags = (uint)MouseFlag.Move,
            X = x,
            Y = y,
        };

        DispatchInput();
    }

    public void SendMouseKey(MouseButtons button, KeyPressMode mode)
    {
        if (mode.HasFlag(KeyPressMode.Down))
            SendMouseKey(button);
        if (mode.HasFlag(KeyPressMode.Up))
            SendMouseKey(button, true);
    }

    public void SendMouseKey(MouseButtons button, bool up = false)
    {
        var user32Button = up ? MouseButtonsToFlags[button].Up : MouseButtonsToFlags[button].Down;

        _buffer[0].Type = (uint)InputType.Mouse;

        _buffer[0].Data.Mouse = new MouseInput
        {
            Flags = (uint)user32Button,
        };

        DispatchInput();
    }

    public void SendScroll(int scrollAmount)
    {
        _buffer[0].Type = (uint)InputType.Mouse;

        _buffer[0].Data.Mouse = new MouseInput
        {
            Flags = (uint)MouseFlag.VerticalWheel,
            MouseData = (uint)scrollAmount,
        };

        DispatchInput();
    }
}