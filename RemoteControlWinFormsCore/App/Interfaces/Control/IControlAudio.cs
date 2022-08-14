namespace RemoteControl.App.Interfaces.Control
{
    public interface IControlAudio
    {
        int Volume { get; set; }
        void Mute(bool mute);
        IReadOnlyCollection<IAudioDevice> GetDevices();
        void SetDevice(Guid id);
    }

    public interface IAudioDevice
    {
        Guid Id { get; set; }
        string Name { get; set; }
        bool IsActive { get; set; }
        bool IsDefault { get; set; }
    }
}
