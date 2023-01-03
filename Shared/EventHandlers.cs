using Shared.Config;
using System.Collections.Generic;

namespace Shared;

public delegate void EmptyEventHandler();
public delegate void BoolEventHandler(bool value);
public delegate void ConfigEventHandler(IEnumerable<CommonConfig> config);
public delegate void StringEventHandler(string? value);
public delegate void IntEventHandler(int? value);