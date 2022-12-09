using System;
using System.Collections.Generic;
using Shared.Controllers.Results;

namespace Shared.Controllers;

public class ControllerMethods : Dictionary<string, Func<string?, IActionResult>>
{
}