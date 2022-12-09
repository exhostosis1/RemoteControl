using Shared.Config;
using Shared.Logging.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Shared;

public interface IContainer
{
    public IAutostartService AutostartService { get; }
    public IConfigProvider ConfigProvider { get; }
    public IList<IControlProcessor> ControlProcessors { get; }
    public IUserInterface UserInterface { get; }
    public ILogger Logger { get; }

    public IControlProcessor? GetProcessorByName(string name) => ControlProcessors.FirstOrDefault(x => x.Name == name);
}