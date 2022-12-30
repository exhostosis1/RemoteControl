namespace Shared.ControlProviders;

public class ControlFacade
{
    public IAudioControlProvider Audio { get; init; }
    public IKeyboardControlProvider Keyboard { get; init; }
    public IMouseControlProvider Mouse { get; init; }
    public IDisplayControlProvider Display { get; init; }

    public ControlFacade(IAudioControlProvider audio, IKeyboardControlProvider keyboard, IMouseControlProvider mouse, IDisplayControlProvider display)
    {
        Audio = audio;
        Keyboard = keyboard;
        Mouse = mouse;
        Display = display;
    }
}
