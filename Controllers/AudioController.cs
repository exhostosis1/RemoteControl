using Shared.Controllers;
using Shared.Controllers.Attributes;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;
using System.Text.Json;

namespace Controllers;

[Controller(MethodNames.AudioControllerName)]
public class AudioController: BaseController
{
    private readonly IAudioControlProvider _audio;

    public AudioController(IAudioControlProvider audio, ILogger logger) : base(logger)
    {
        _audio = audio;
    }

    [Action(MethodNames.AudioGetDevice)]
    public string? AudioDevice(string? _)
    {
        return JsonSerializer.Serialize(_audio.GetDevices());
    }

    [Action(MethodNames.AudioSetDevice)]
    public string? SetDevice(string? param)
    {
        return Guid.TryParse(param, out var guid) ? 
            JsonSerializer.Serialize(_audio.SetCurrentControlDevice(guid)) : 
            "error";
    }

    [Action(MethodNames.AudioGetVolume)]
    public string? GetVolume(string? _)
    {
        return _audio.GetVolume().ToString();
    }

    [Action(MethodNames.AudioSetVolume)]
    public string? SetVolume(string? param)
    {
        if (!int.TryParse(param, out var result)) return "error";

        result = result > 100 ? 100 : result < 0 ? 0 : result;

        _audio.SetVolume(result);

        return "done";
    }
}