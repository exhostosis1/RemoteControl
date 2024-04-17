namespace MainApp.ControlProviders.Interfaces;

internal interface IAudioControl
{
    public int GetVolume();
    public void SetVolume(int volume);
    void Mute();
    void Unmute();
    bool IsMuted { get; }
    IEnumerable<IAudioDevice> GetAudioDevices();
    void SetAudioDevice(Guid id);
}