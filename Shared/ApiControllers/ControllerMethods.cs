using System;
using System.Collections.Generic;
using Shared.ApiControllers.Results;

namespace Shared.ApiControllers;

public class ControllerMethods : Dictionary<string, Func<string?, IActionResult>>
{
    public ControllerMethods(){}

    public ControllerMethods(IDictionary<string, Func<string?, IActionResult>> input) : base(input)
    {
    }
}