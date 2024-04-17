namespace MainApp.ControlProviders.Enums;

[Flags]
internal enum KeyPressMode
{
    Down = 0b01,
    Up = 0b10,
    Click = Down | Up
}