using System.Collections.Generic;

namespace Shared.Controllers;

public class ControllersWithMethods: Dictionary<string, ControllerMethods>
{
    public new ControllerMethods this[string index] => this.ContainsKey(index) ? this[index] : new ControllerMethods();
}