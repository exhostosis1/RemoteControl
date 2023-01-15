namespace Shared.AudioWrapper;

public interface IMMDevice
{
    public string ID { get; }
    public string DeviceFriendlyName { get; }
    public IAudioEndpointVolume AudioEndpointVolume { get; }
}