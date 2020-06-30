namespace RemoteControl.Core.Interfaces
{
    internal interface IAudioService
    {
        int Volume { get; set; }
        void Mute(bool mute);
    }
}
