using Shared.Config;
using Shared.Logging.Interfaces;
using System.Collections.Generic;

namespace Shared;

public interface IContainer
{
    public IAutostartService AutostartService { get; }
    public IConfigProvider ConfigProvider { get; }
    public ICollection<IControlProcessor> ControlProcessors { get; }
    public IUserInterface UserInterface { get; }
    public ILogger Logger { get; }
}