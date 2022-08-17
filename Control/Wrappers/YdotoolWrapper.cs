using Shared.Enums;
using Shared.Interfaces.Control;
using Shared.Interfaces.Logging;

namespace Control.Wrappers
{
    public class YdotoolWrapper: BaseWrapper, IKeyboardControl, IMouseControl
    {
        public YdotoolWrapper(ILogger logger): base(logger)
        {}

        public void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click)
        {
            Logger.LogInfo(key.ToString());
        }

        public void TextInput(string text)
        {
            Logger.LogInfo(text);
        }

        public void Move(int x, int y)
        {
            Logger.LogInfo($"{x} - {y}");
        }

        public void ButtonPress(MouseKeysEnum key = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click)
        {
            Logger.LogInfo(key.ToString());
        }

        public void Wheel(bool up)
        {
            Logger.LogInfo(up.ToString());
        }
    }
}
