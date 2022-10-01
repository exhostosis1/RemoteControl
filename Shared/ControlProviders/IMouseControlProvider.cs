using Shared.Enums;

namespace Shared.ControlProviders;

public interface IMouseControlProvider
{
    void Move(int x, int y);
    void ButtonPress(MouseButtons button = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click);
    void Wheel(bool up);
}