using RemoteControl.Core.Interfaces;
using RemoteControl.Core.Providers;

namespace RemoteControl.Core.Services
{
    internal class AudioService : IAudioService
    {
        private readonly IAudioProvider _provider = new AudioSwitcherProvider();

        public int Volume { get => _provider.GetVolume(); set => _provider.SetVolume(value); }

        public void Mute(bool mute) => _provider.Mute(mute);
    }
}
