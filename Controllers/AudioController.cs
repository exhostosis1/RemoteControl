using Shared.Controllers;
using Shared.Controllers.Results;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;

namespace Controllers;

public class AudioController: BaseController
{
    private readonly IAudioControlProvider _audio;

    public AudioController(IAudioControlProvider audio, ILogger logger) : base(logger)
    {
        _audio = audio;
    }

    public IActionResult GetDevices(string? _)
    {
        return Json(_audio.GetDevices());
    }

    public IActionResult SetDevice(string? param)
    {
        if(Guid.TryParse(param, out var guid)) 
        {
            _audio.SetCurrentControlDevice(guid);
            return Ok();
        }

        return Error("No such device");
    }

    public IActionResult GetVolume(string? _)
    {
        return Text(_audio.GetVolume());
    }

    public IActionResult SetVolume(string? param)
    {
        if (!int.TryParse(param, out var result)) return Error("Wrong volume format");

        result = result > 100 ? 100 : result < 0 ? 0 : result;

        _audio.SetVolume(result);

        return Text(result);
    }

    public IActionResult IncreaseBy5(string? _)
    {
        var vol = _audio.GetVolume();
        vol += 5;
        vol = vol > 100 ? 100 : vol;

        _audio.SetVolume(vol);

        return Text(vol);
    }

    public IActionResult DecreaseBy5(string? _)
    {
        var vol = _audio.GetVolume();
        vol -= 5;
        vol = vol < 0 ? 0 : vol;

        _audio.SetVolume(vol);

        return Text(vol);
    }

    public IActionResult Mute(string? _)
    {
        if(_audio.IsMuted)
            _audio.Unmute();
        else
            _audio.Mute();

        return Ok();
    }
}