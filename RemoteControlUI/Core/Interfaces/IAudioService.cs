namespace RemoteControl.Core.Interfaces
{
    internal interface IAudioService
    {
        void SetVolume(int volume);
        string GetVolume();
        void Mute(bool mute);
    }
}
