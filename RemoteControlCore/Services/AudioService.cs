using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using RemoteControlCore.Interfaces;
using System;
using System.Threading.Tasks;

namespace RemoteControlCore.Services
{
    internal class AudioService : IAudioService, IObserver<DeviceChangedArgs>
    {
        IDevice _audioDevice;

        public AudioService()
        {
            var audioController = new CoreAudioController();

            Task.Run(async () => _audioDevice = await audioController.GetDefaultDeviceAsync(DeviceType.Playback, Role.Multimedia)).Wait();
            audioController.AudioDeviceChanged.Subscribe(this);
        }

        public string GetVolume()
        {
            return _audioDevice.Volume.ToString();
        }

        public void Mute(bool mute)
        {
            _audioDevice.Mute(mute);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw error;
        }

        public void OnNext(DeviceChangedArgs value)
        {
            if (value.ChangedType == DeviceChangedType.DefaultChanged && value.Device.DeviceType == DeviceType.Playback)
            {
                _audioDevice = value.Device;
            }
        }

        public void SetVolume(int volume)
        {
            _audioDevice.Volume = volume;
        }
    }
}
