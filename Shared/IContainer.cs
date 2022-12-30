using Shared.Config;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;

namespace Shared;

public interface IContainer
{
    public IConfigProvider ConfigProvider { get; }
    public IAutostartService AutostartService { get; }
    public ILogger Logger { get; }
    public IUserInterface UserInterface { get; set; }
    public ControlFacade ControlProviders { get; set; }
}