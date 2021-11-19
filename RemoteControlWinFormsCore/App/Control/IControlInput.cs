using RemoteControl.App.Enums;
using RemoteControl.App.Utility;

namespace RemoteControl.App.Control.Interfaces
{
    internal interface IControlInput
    {
        void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click);
        void TextInput(string text);
        void MouseMove(MyPoint coords);
        void MouseKeyPress(MouseKeysEnum key = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click);
        void MouseWheel(bool up);
    }
}
