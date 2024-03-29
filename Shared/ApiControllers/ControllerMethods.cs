using Shared.ApiControllers.Results;
using System;
using System.Collections.Generic;

namespace Shared.ApiControllers;

public class ControllerMethods(IDictionary<string, Func<string?, IActionResult>> input)
    : Dictionary<string, Func<string?, IActionResult>>(input);