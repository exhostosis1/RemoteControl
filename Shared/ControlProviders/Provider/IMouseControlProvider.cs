using Shared.Enums;

namespace Shared.ControlProviders.Provider;

public interface IMouseControlProvider
{
    void MouseMove(int x, int y);
    void MouseKeyPress(MouseButtons button = MouseButtons.Left, KeyPressMode mode = KeyPressMode.Click);
    void MouseWheel(bool up);
}