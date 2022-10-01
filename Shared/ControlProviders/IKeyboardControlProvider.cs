using Shared.Enums;

namespace Shared.ControlProviders;

public interface IKeyboardControlProvider
{
    void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click);
    void TextInput(string text);
}