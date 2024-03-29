using System.Collections.Generic;

namespace Shared.ApiControllers;

public class ControllersWithMethods(IDictionary<string, ControllerMethods> input)
    : Dictionary<string, ControllerMethods>(input);