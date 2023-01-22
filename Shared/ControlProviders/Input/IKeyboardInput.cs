using Shared.Enums;

namespace Shared.ControlProviders.Input;

public interface IKeyboardInput
{
    public void SendKey(KeysEnum key, KeyPressMode mode);
    public void SendText(string text);
}