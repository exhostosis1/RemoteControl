using RemoteControlCore.Enums;

namespace RemoteControlCore.Interfaces
{
    internal interface IInputProvider
    {
        IKeyboard Keyboard { get; }
        IMouse Mouse { get; }
    }

    internal interface IKeyboard
    {
        void KeyUp(KeysEnum key);
        void KeyDown(KeysEnum key);
        void KeyPress(KeysEnum key);
        void TextEntry(string text);
    }

    internal interface IMouse
    {
        void MouseButtonUp(MouseKeysEnum key);
        void MouseButtonDown(MouseKeysEnum key);
        void MouseButtonClick(MouseKeysEnum key);
        void MoveMouseBy(int x, int y);
        void VerticalScroll(int y);
    }
}
