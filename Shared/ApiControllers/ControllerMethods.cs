using Shared.ApiControllers.Results;
using System;
using System.Collections.Generic;

namespace Shared.ApiControllers;

public class ControllerMethods : Dictionary<string, Func<string?, IActionResult>>
{
    public ControllerMethods() { }

    public ControllerMethods(IDictionary<string, Func<string?, IActionResult>> input) : base(input)
    {
    }
}