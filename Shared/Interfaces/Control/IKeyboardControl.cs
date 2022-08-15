using Shared.Enums;

namespace Shared.Interfaces.Control
{
    public interface IKeyboardControl
    {
        void KeyPress(KeysEnum key, KeyPressMode mode = KeyPressMode.Click);
        void TextInput(string text);
    }
}
