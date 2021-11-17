using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;
using RemoteControl.Core.Interfaces;

namespace RemoteControl.Core.Providers
{
    internal class AudioSwitcherProvider: IAudioProvider 
    {
        private IDevice _audioDevice;

        public int GetVolume() => (int)_audioDevice.Volume;
        public void SetVolume(int volume) => _audioDevice.SetVolumeAsync(volume);        

        public AudioSwitcherProvider()
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
