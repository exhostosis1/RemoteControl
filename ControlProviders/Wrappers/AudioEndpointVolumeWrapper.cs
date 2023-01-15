using NAudio.CoreAudioApi;
using Shared.AudioWrapper;

namespace ControlProviders.Wrappers;

public class AudioEndpointVolumeWrapper: IAudioEndpointVolume
{
    private readonly AudioEndpointVolume _volume;

    public AudioEndpointVolumeWrapper(AudioEndpointVolume volume)
    {
        _volume = volume;
    }

    public float MasterVolumeLevelScalar
    {
        get => _volume.MasterVolumeLevelScalar; 
        set => _volume.MasterVolumeLevelScalar = value;
    }

    public bool Mute
    {
        get => _volume.Mute; 
        set => _volume.Mute = value;
    }
}