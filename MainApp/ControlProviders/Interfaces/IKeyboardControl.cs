using MainApp.ControlProviders.Enums;

namespace MainApp.ControlProviders.Interfaces;

internal interface IKeyboardControl
{
    void KeyboardKeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click);
    void TextInput(string text);
}