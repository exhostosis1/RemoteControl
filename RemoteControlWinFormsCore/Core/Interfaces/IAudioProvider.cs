namespace RemoteControl.Core.Interfaces
{
    internal interface IAudioProvider
    {
        int GetVolume();

        void SetVolume(int volume);

        void Mute(bool mute);
    }
}
