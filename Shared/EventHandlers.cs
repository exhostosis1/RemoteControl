using Shared.Config;

namespace Shared;

public delegate void EmptyEventHandler();
public delegate void BoolEventHandler(bool value);
public delegate void ConfigWithIdEventHandler(int id, CommonConfig config);
public delegate void ConfigEventHandler(CommonConfig config);
public delegate void StringEventHandler(string value);
public delegate void NullableStringEventHandler(string? value);
public delegate void IntEventHandler(int value);
public delegate void NullableIntEventHandler(int? value);