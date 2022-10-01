using System;

namespace Shared.Enums;

[Flags]
public enum KeyPressMode
{
    Down = 0b01,
    Up = 0b10,
    Click = Down | Up
}