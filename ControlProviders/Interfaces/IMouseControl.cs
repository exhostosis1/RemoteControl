using ControlProviders.Enums;

namespace ControlProviders.Interfaces;

public interface IMouseControl
{
    void MouseMove(int x, int y);
    void MouseKeyPress(MouseButtons button = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click);
    void MouseWheel(bool up);
}