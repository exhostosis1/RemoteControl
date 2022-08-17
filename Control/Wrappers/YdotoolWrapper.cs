using Shared.Enums;
using Shared.Interfaces.Control;
using Shared.Interfaces.Logging;

namespace Control.Wrappers
{
    public class YdotoolWrapper : BaseWrapper, IKeyboardControl, IMouseControl
    {
        public YdotoolWrapper(ILogger logger) : base(logger)
        { }

        enum YdotoolKey: byte
        {
            KEY_RESERVED = 0,
            KEY_ESC = 115,
            KEY_1 = 2,
            KEY_2 = 3,
            KEY_3 = 4,
            KEY_4 = 5,
            KEY_5 = 6,
            KEY_6 = 7,
            KEY_7 = 8,
            KEY_8 = 9,
            KEY_9 = 10,
            KEY_0 = 11,
            KEY_MINUS = 12,
            KEY_EQUAL = 13,
            KEY_BACKSPACE = 14,
            KEY_TAB = 15,
            KEY_Q = 16,
            KEY_W = 17,
            KEY_E = 18,
            KEY_R = 19,
            KEY_T = 20,
            KEY_Y = 21,
            KEY_U = 22,
            KEY_I = 23,
            KEY_O = 24,
            KEY_P = 25,
            KEY_LEFTBRACE = 26,
            KEY_RIGHTBRACE = 27,
            KEY_ENTER = 28,
            KEY_LEFTCTRL = 29,
            KEY_A = 30,
            KEY_S = 31,
            KEY_D = 32,
            KEY_F = 33,
            KEY_G = 34,
            KEY_H = 35,
            KEY_J = 36,
            KEY_K = 37,
            KEY_L = 38,
            KEY_SEMICOLON = 39,
            KEY_APOSTROPHE = 40,
            KEY_GRAVE = 41,
            KEY_LEFTSHIFT = 42,
            KEY_BACKSLASH = 43,
            KEY_Z = 44,
            KEY_X = 45,
            KEY_C = 46,
            KEY_V = 47,
            KEY_B = 48,
            KEY_N = 49,
            KEY_M = 50,
            KEY_COMMA = 51,
            KEY_DOT = 52,
            KEY_SLASH = 53,
            KEY_RIGHTSHIFT = 54,
            KEY_KPASTERISK = 55,
            KEY_LEFTALT = 56,
            KEY_SPACE = 57,
            KEY_CAPSLOCK = 58,
            KEY_F1 = 59,
            KEY_F2 = 60,
            KEY_F3 = 61,
            KEY_F4 = 62,
            KEY_F5 = 63,
            KEY_F6 = 64,
            KEY_F7 = 65,
            KEY_F8 = 66,
            KEY_F9 = 67,
            KEY_F10 = 68,
            KEY_NUMLOCK = 69,
            KEY_SCROLLLOCK = 70,
            KEY_F11 = 87,
            KEY_F12 = 88,
            KEY_SYSRQ = 99,
            KEY_RIGHTALT = 100,
            KEY_HOME = 102,
            KEY_UP = 103,
            KEY_PAGEUP = 104,
            KEY_LEFT = 105,
            KEY_RIGHT = 106,
            KEY_END = 107,
            KEY_DOWN = 108,
            KEY_PAGEDOWN = 109,
            KEY_INSERT = 110,
            KEY_DELETE = 111,
            KEY_NEXTSONG = 163,
            KEY_PLAYPAUSE = 164,
            KEY_PREVIOUSSONG = 165,
        }

        enum MouseCodes: byte
        {
            LEFT = 0x00,
            RIGHT = 0x01,
            MIDDLE = 0x02,
            SIDE = 0x03,
            EXTR = 0x04,
            FORWARD = 0x05,
            BACK = 0x06,
            TASK = 0x07,
            Down = 0x40,
            Up = 0x80
        }

        private readonly Dictionary<KeysEnum, YdotoolKey> KeyToScanCode = new()
        {
            {
                KeysEnum.ArrowLeft, YdotoolKey.KEY_LEFT
            },
            {
                KeysEnum.ArrowRight, YdotoolKey.KEY_RIGHT
            },
            {
                KeysEnum.MediaNext, YdotoolKey.KEY_NEXTSONG
            },
            {
                KeysEnum.MediaPrev, YdotoolKey.KEY_PREVIOUSSONG
            },
            {
                KeysEnum.MediaPlayPause, YdotoolKey.KEY_PLAYPAUSE
            },
            {
                KeysEnum.Enter, YdotoolKey.KEY_ENTER
            },
        };

        private readonly Dictionary<MouseButtons, MouseCodes> ButtonToCode = new()
        {
            {
                MouseButtons.Left, MouseCodes.LEFT
            },
            {
                MouseButtons.Right, MouseCodes.RIGHT
            },
            {
                MouseButtons.Middle, MouseCodes.MIDDLE
            }
        };

        private (string, string) RunLinuxCommand(string command)
        {
            using var proc = new System.Diagnostics.Process();

            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.Arguments = "-c \" " + command + " \"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;

            proc.Start();

            var result = proc.StandardOutput.ReadToEnd();
            var error = proc.StandardError.ReadToEnd();

            proc.WaitForExit();

            return (result.Trim(), error.Trim());
        }

        private (string, string) RunYdotool(string args)
        {
            return RunLinuxCommand($"ydotool {args}");
        }

        private (string, string) SendKey(YdotoolKey key)
        {
            return RunYdotool($"key {(byte)key}:1 {(byte)key}:0");
        }

        private (string, string) SendText(string text)
        {
            return RunYdotool($"type {text}");
        }

        private (string, string) SendMouseMove(int x, int y)
        {
            return RunYdotool($"mousemove -- {x} {y}");
        }

        private (string, string) SendMouseButton(MouseCodes button)
        {
            return RunYdotool($"click {((byte)button):x}");
        }

        public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click)
        {
            var (result, error) = SendKey(KeyToScanCode[key]);

            if (!string.IsNullOrEmpty(error))
            {
                Logger.LogError(error);
            }
            else if (!string.IsNullOrEmpty(result))
            {
                Logger.LogInfo(result);
            }
        }

        public void TextInput(string text)
        {
            var (result, error) = SendText(text);

            if (!string.IsNullOrEmpty(error))
            {
                Logger.LogError(error);
            }
            else if (!string.IsNullOrEmpty(result))
            {
                Logger.LogInfo(result);
            }
        }

        public void Move(int x, int y)
        {
            var (result, error) = SendMouseMove(x, y);

            if (!string.IsNullOrEmpty(error))
            {
                Logger.LogError(error);
            }
            else if (!string.IsNullOrEmpty(result))
            {
                Logger.LogInfo(result);
            }
        }

        public void ButtonPress(MouseButtons button = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click)
        {
            MouseCodes mouseCode;

            if(mode == KeyPressMode.Click)
            {
                mouseCode = ButtonToCode[button] | MouseCodes.Down | MouseCodes.Up;
            }
            else
            {
                mouseCode = ButtonToCode[button] | (mode == KeyPressMode.Up ? MouseCodes.Up : MouseCodes.Up);
            }

            var (result, error) = SendMouseButton(mouseCode);

            if (!string.IsNullOrEmpty(error))
            {
                Logger.LogError(error);
            }
            else if (!string.IsNullOrEmpty(result))
            {
                Logger.LogInfo(result);
            }
        }

        public void Wheel(bool up)
        {
            Logger.LogInfo(up.ToString());
        }
    }
}
