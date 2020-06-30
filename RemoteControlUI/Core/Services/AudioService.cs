using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;
using RemoteControl.Core.Interfaces;

namespace RemoteControl.Core.Services
{
    internal class AudioService : IAudioService
    {
        private IDevice _audioDevice;

        public int Volume
        {
            get => (int) _audioDevice.Volume;
            set => _audioDevice.SetVolumeAsync(value);
        }

        public AudioService()
        {
            var audioController = new CoreAudioController();

            _audioDevice = audioController.GetDefaultDevice(DeviceType.Playback, Role.Multimedia);
            audioController.AudioDeviceChanged
                .When(x => x.ChangedType == DeviceChangedType.DefaultChanged && x.Device.IsPlaybackDevice)
                .Subscribe(x => _audioDevice = x.Device);
        }

        public void Mute(bool mute) => _audioDevice.SetMuteAsync(mute);
    }
}
