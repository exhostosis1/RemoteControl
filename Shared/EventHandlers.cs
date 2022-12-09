namespace Shared;

public delegate void EmptyEventHandler();
public delegate void BoolEventHandler(bool value);
public delegate void ConfigEventHandler((string Name, string Info) config);
public delegate void StringEventHandler(string? value);