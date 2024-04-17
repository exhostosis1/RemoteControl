namespace MainApp.ControlProviders.Interfaces;

internal interface IAudioDevice
{
    Guid Id { get; set; }
    string Name { get; set; }
    bool IsCurrentControlDevice { get; set; }
}