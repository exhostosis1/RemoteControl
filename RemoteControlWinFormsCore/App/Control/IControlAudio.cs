namespace RemoteControl.App.Control.Interfaces
{
    internal interface IControlAudio
    {
        int Volume { get; set; }
        void Mute(bool mute);
    }
}
