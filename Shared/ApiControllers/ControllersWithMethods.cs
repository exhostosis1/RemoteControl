using System.Collections.Generic;

namespace Shared.ApiControllers;

public class ControllersWithMethods : Dictionary<string, ControllerMethods>
{
    public ControllersWithMethods() { }

    public ControllersWithMethods(IDictionary<string, ControllerMethods> input) : base(input) { }
}