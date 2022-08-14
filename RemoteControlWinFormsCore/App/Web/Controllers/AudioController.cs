﻿using RemoteControl.App.Interfaces.Control;
using System.Text.Json;
using RemoteControl.App.Web.Attributes;

namespace RemoteControl.App.Web.Controllers
{
    [Controller("audio")]
    internal class AudioController: BaseController
    {
        private readonly IAudioControl _audio;

        public AudioController(IAudioControl audio)
        {
            _audio = audio;
        }

        [Action("getdevice")]
        public string? AudioDevice(string _)
        {
            return JsonSerializer.Serialize(_audio.GetDevices());
        }

        [Action("setdevice")]
        public string? SetDevice(string param)
        {
            if (Guid.TryParse(param, out var guid))
            {
                _audio.SetDevice(guid);
                return ("done");
            }

            return "error";
        }

        [Action("getvolume")]
        public string? GetVolume(string _)
        {
            return _audio.Volume.ToString();
        }

        [Action("setvolume")]
        public string? SetVolume(string param)
        {
            if (!int.TryParse(param, out var result)) return "error";

            result = result > 100 ? 100 : result < 0 ? 0 : result;

            _audio.Volume = result;
            _audio.Mute(result == 0);

            return "done";
        }
    }
}
