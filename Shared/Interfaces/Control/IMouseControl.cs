using Shared.Enums;

namespace Shared.Interfaces.Control;

public interface IMouseControl
{
    void Move(int x, int y);
    void ButtonPress(MouseButtons button = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click);
    void Wheel(bool up);
}