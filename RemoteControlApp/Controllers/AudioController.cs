using System.Text.Json;
using Shared.Control;
using Shared.Controllers;
using Shared.Controllers.Attributes;
using Shared.Logging.Interfaces;

namespace RemoteControlApp.Controllers
{
    [Controller("audio")]
    public class AudioController: BaseController
    {
        private readonly IAudioControl _audio;

        public AudioController(IAudioControl audio, ILogger logger) : base(logger)
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
            return Guid.TryParse(param, out var guid) ? 
                JsonSerializer.Serialize(_audio.SetCurrentControlDevice(guid)) : 
                "error";
        }

        [Action("getvolume")]
        public string? GetVolume(string _)
        {
            return _audio.GetVolume().ToString();
        }

        [Action("setvolume")]
        public string? SetVolume(string param)
        {
            if (!int.TryParse(param, out var result)) return "error";

            result = result > 100 ? 100 : result < 0 ? 0 : result;

            _audio.SetVolume(result);

            return "done";
        }
    }
}
