using System;
using System.Collections.Generic;

namespace Shared.Controllers;

public class ControllerMethods : Dictionary<string, Func<string?, string?>>
{
    public new Func<string?, string?> this[string index]
    {
        get
        {
            return this.ContainsKey(index) ? this[index] : _ => null;
        }
    }
}