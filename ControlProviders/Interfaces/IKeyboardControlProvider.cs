using ControlProviders.Enums;

namespace ControlProviders.Interfaces;

public interface IKeyboardControlProvider
{
    void KeyboardKeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click);
    void TextInput(string text);
}