namespace RemoteControl.Core.Interfaces
{
    internal interface IAudioService
    {
        void SetVolume(int volume);
        int GetVolume();
        void Mute(bool mute);
    }
}
