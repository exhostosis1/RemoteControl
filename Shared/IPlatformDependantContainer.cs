using System;
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
    public ControlFacade ControlProviders { get; }

    public ILogger GetLogger(Type type);
}