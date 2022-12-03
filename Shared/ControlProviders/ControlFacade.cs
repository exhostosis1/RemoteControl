namespace Shared.ControlProviders;

public class ControlFacade
{
    public IKeyboardControlProvider KeyboardControlProvider { get; set; }
    public IMouseControlProvider MouseControlProvider { get; set; }
    public IDisplayControlProvider DisplayControlProvider { get; set; }
    public IAudioControlProvider AudioControlProvider { get; set; }
}
