using Shared.Config;

namespace Shared;

public delegate void EmptyEventHandler();
public delegate void BoolEventHandler(bool value);
public delegate void ConfigEventHandler(int index, CommonConfig config);
public delegate void ProcessorEventHandler(CommonConfig config);
public delegate void StringEventHandler(string? value);
public delegate void IntEventHandler(int? value);