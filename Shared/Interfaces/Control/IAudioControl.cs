namespace Shared.Interfaces.Control
{
    public interface IAudioControl
    {
        public int GetVolume();
        public void SetVolume(int volume);
        void Mute(bool mute);
        IReadOnlyCollection<IAudioDevice> GetDevices();
        IReadOnlyCollection<IAudioDevice> SetCurrentControlDevice(Guid id);
    }
}
