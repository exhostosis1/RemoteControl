using System;
using System.Collections.Generic;
using Shared.Controllers.Results;

namespace Shared.Controllers;

public class ControllerMethods : Dictionary<string, Func<string?, IActionResult>>
{
    public ControllerMethods(){}

    public ControllerMethods(IDictionary<string, Func<string?, IActionResult>> input) : base(input)
    {
    }
}