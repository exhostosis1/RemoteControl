using System;

namespace Shared.AudioWrapper;

[Flags]
public enum DeviceState
{
    Active = 1,
    Disabled = 2,
    NotPresent = 4,
    Unplugged = 8,
    All = Active | Disabled | NotPresent | Unplugged
}