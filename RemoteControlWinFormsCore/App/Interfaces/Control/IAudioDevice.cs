namespace RemoteControl.App.Interfaces.Control;

public interface IAudioDevice
{
    Guid Id { get; set; }
    string Name { get; set; }
    bool IsActive { get; set; }
    bool IsDefault { get; set; }
}