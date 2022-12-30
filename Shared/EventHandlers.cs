using System.Collections.Generic;
using Shared.Enums;

namespace Shared;

public delegate void EmptyEventHandler();
public delegate void BoolEventHandler(bool value);
public delegate void ConfigEventHandler(IEnumerable<ControlProcessorDto> config);
public delegate void StringEventHandler(string? value);
public delegate void ProcessorEventHandler(string? name, ControlProcessorType type);