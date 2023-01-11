using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;
using Shared.UI;

namespace Shared;

public interface IPlatformDependantContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public IUserInterface UserInterface { get; }
    public IAudioControlProvider AudioProvider { get; }
    public IDisplayControlProvider DisplayProvider { get; }
    public IKeyboardControlProvider KeyboardProvider { get; }
    public IMouseControlProvider MouseProvider { get; }
    public ControlFacade ControlProviders { get; }
    public ILogger Logger { get; }

    public ILogger NewLogger();
    public IConfigProvider NewConfigProvider(ILogger logger);
    public IAutostartService NewAutostartService(ILogger logger);
    public IUserInterface NewUserInterface();
    public IKeyboardControlProvider NewKeyboardProvider(ILogger logger);
    public IMouseControlProvider NewMouseProvider(ILogger logger);
    public IDisplayControlProvider NewDisplayProvider(ILogger logger);
    public IAudioControlProvider NewAudioProvider(ILogger logger);
}