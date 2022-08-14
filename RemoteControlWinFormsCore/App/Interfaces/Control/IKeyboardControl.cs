using RemoteControl.App.Enums;

namespace RemoteControl.App.Interfaces.Control
{
    public interface IKeyboardControl
    {
        void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click);
        void TextInput(string text);
    }
}
