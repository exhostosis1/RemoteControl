using RemoteControlCore.Enums;

namespace RemoteControlCore.Interfaces
{
    internal interface IInputProvider
    {
        IKeyboard Keyboard { get; set; }
        IMouse Mouse { get; set; }
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
        void MoveMouseBy(int X, int Y);
        void VerticalScroll(int Y);
    }
}
