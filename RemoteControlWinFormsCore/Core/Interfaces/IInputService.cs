using RemoteControl.Core.Enums;
using RemoteControl.Core.Utility;

namespace RemoteControl.Core.Interfaces
{
    internal interface IInputService
    {
        void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click);
        void TextInput(string text);
        void MouseMove(MyPoint coords);
        void MouseKeyPress(MouseKeysEnum key = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click);
        void MouseWheel(bool up);
    }
}
