using RemoteControl.App.Enums;

namespace RemoteControl.App.Interfaces.Control;

public interface IMouseControl
{
    void Move(int x, int y);
    void KeyPress(MouseKeysEnum key = MouseKeysEnum.Left, KeyPressMode mode = KeyPressMode.Click);
    void Wheel(bool up);
}