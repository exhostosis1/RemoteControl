using MainApp.ControlProviders.Interfaces;

namespace MainApp.ControlProviders;

internal class ControlProvider
{
    private NAudioWrapper? _audioWrapper;
    private User32Wrapper? _user32Wrapper;

    public IAudioControl GetAudioControl()
    {
        _audioWrapper ??= new NAudioWrapper();
        return _audioWrapper;
    }

    public IMouseControl GetMouseControl()
    {
        _user32Wrapper ??= new User32Wrapper();
        return _user32Wrapper;
    }

    public IKeyboardControl GetKeyboardControl()
    {
        _user32Wrapper ??= new User32Wrapper();
        return _user32Wrapper;
    }

    public IDisplayControl GetDisplayControl()
    {
        _user32Wrapper ??= new User32Wrapper();
        return _user32Wrapper;
    }
}