using RemoteControl.Core.Enums;

namespace RemoteControl.Core.Interfaces
{
    internal interface IInputService
    {
        void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click);
        void TextInput(string text);
        void MouseMove(ICoordinates coords);
        void MouseKeyPress(MouseKeysEnum key = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click);
        void MouseWheel(bool up);
    }
}
