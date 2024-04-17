using MainApp.ControlProviders.Interfaces;

namespace MainApp.ControlProviders;

internal class AudioDevice: IAudioDevice
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsCurrentControlDevice { get; set; }
}