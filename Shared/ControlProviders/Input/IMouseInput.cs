using Shared.Enums;

namespace Shared.ControlProviders.Input;

public interface IMouseInput
{
    public void SendMouseKey(MouseButtons key, KeyPressMode mode);
    public void SendMouseMove(int x, int y);
    public void SendScroll(int scrollAmount);
}