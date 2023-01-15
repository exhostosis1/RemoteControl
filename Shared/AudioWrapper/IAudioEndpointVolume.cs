namespace Shared.AudioWrapper;

public interface IAudioEndpointVolume
{
    public float MasterVolumeLevelScalar { get; set; }
    public bool Mute { get; set; }
}