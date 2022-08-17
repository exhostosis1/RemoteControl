namespace Shared.Interfaces.Control
{
    public interface IAudioControl
    {
        public int GetVolume();
        public void SetVolume(int volume);
        void Mute();
        void Unmute();
        IReadOnlyCollection<IAudioDevice> GetDevices();
        IReadOnlyCollection<IAudioDevice> SetCurrentControlDevice(Guid id);
    }
}
