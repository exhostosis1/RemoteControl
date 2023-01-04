using System.Collections.Generic;

namespace Shared.Controllers;

public class ControllersWithMethods: Dictionary<string, ControllerMethods>
{
    public ControllersWithMethods(){}

    public ControllersWithMethods(IDictionary<string, ControllerMethods> input): base(input){}
}