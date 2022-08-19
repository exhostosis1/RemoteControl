using Shared.Enums;

namespace Shared.Control
{
    public interface IKeyboardControl
    {
        void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click);
        void TextInput(string text);
    }
}
