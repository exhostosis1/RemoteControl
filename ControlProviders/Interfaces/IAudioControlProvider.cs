namespace ControlProviders.Interfaces;

public interface IAudioControlProvider
{
    public int GetVolume();
    public void SetVolume(int volume);
    void Mute();
    void Unmute();
    bool IsMuted { get; }
    IEnumerable<IAudioDevice> GetAudioDevices();
    void SetAudioDevice(Guid id);
}