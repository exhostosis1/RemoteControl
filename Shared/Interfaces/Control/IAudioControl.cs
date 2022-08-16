namespace Shared.Interfaces.Control
{
    public interface IAudioControl
    {
        int Volume { get; set; }
        void Mute(bool mute);
        IReadOnlyCollection<IAudioDevice> GetDevices();
        IReadOnlyCollection<IAudioDevice> SetCurrentControlDevice(Guid id);
    }
}
