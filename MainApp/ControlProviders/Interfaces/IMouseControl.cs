using MainApp.ControlProviders.Enums;

namespace MainApp.ControlProviders.Interfaces;

internal interface IMouseControl
{
    void MouseMove(int x, int y);
    void MouseKeyPress(MouseButtons button = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click);
    void MouseWheel(bool up);
}