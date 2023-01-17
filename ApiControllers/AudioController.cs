using Shared.ApiControllers.Results;
using Shared.ControlProviders;
using Shared.Logging.Interfaces;

namespace ApiControllers;

public class AudioController: BaseApiController
{
    private readonly IControlProvider _provider;
    private readonly ILogger<AudioController> _logger;

    public AudioController(IControlProvider provider, ILogger<AudioController> logger) : base(logger)
    {
        _logger = logger;
        _provider = provider;
    }

    public IActionResult GetDevices(string? _)
    {
        _logger.LogInfo("Getting devices");

        return Json(_provider.GetAudioDevices());
    }

    public IActionResult SetDevice(string? param)
    {
        _logger.LogInfo($"Setting device to {param}");

        if(Guid.TryParse(param, out var guid)) 
        {
            _provider.SetAudioDevice(guid);
            return Ok();
        }

        _logger.LogError($"Cannot set device to {param}");
        return Error("No such device");
    }

    public IActionResult GetVolume(string? _)
    {
        _logger.LogInfo("Getting volume");

        return Text(_provider.GetVolume());
    }

    public IActionResult SetVolume(string? param)
    {
        _logger.LogInfo($"Setting volume to {param}");

        if (!int.TryParse(param, out var result))
        {
            _logger.LogError($"Cannot set volume to {param}");
            return Error("Wrong volume format");
        }

        result = result > 100 ? 100 : result < 0 ? 0 : result;

        _provider.SetVolume(result);

        return Text(result);
    }

    public IActionResult IncreaseBy5(string? _)
    {
        _logger.LogInfo("Increasing volume by 5");

        var vol = _provider.GetVolume();
        vol += 5;
        vol = vol > 100 ? 100 : vol;

        _provider.SetVolume(vol);

        return Text(vol);
    }

    public IActionResult DecreaseBy5(string? _)
    {
        _logger.LogInfo("Decreasing volume by 5");

        var vol = _provider.GetVolume();
        vol -= 5;
        vol = vol < 0 ? 0 : vol;

        _provider.SetVolume(vol);

        return Text(vol);
    }

    public IActionResult Mute(string? _)
    {
        _logger.LogInfo("Toggling mute status");

        if(_provider.IsMuted)
            _provider.Unmute();
        else
            _provider.Mute();

        return Ok();
    }
}