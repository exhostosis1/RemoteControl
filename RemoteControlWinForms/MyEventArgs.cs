using Shared.Enums;

namespace RemoteControlWinForms;

internal class MyEventArgs : EventArgs
{
    public string? ProcessorName { get; set; }
    public ControlProcessorType ProcessorType { get; set; }
}