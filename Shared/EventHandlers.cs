using Shared.Config;

namespace Shared;

public delegate void EmptyEventHandler();
public delegate void BoolEventHandler(bool value);
public delegate void ConfigWithIndexEventHandler(int index, CommonConfig config);
public delegate void ConfigEventHandler(CommonConfig config);
public delegate void StringEventHandler(string? value);
public delegate void IntEventHandler(int? value);