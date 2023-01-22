using Shared.Enums;

namespace Shared.ControlProviders.Provider;

public interface IKeyboardControlProvider
{
    void KeyboardKeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click);
    void TextInput(string text);
}