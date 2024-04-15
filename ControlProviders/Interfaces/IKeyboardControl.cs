using ControlProviders.Enums;

namespace ControlProviders.Interfaces;

public interface IKeyboardControl
{
    void KeyboardKeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click);
    void TextInput(string text);
}