using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;
using Shared.Logging.Interfaces;

namespace ApiControllers;

public class AudioController(IAudioControlProvider provider, ILogger<AudioController> logger) : BaseApiController
{
    public IActionResult GetDevices(string? _)
    {
        logger.LogInfo("Getting devices");

        return Json(provider.GetAudioDevices());
    }

    public IActionResult SetDevice(string? param)
    {
        logger.LogInfo($"Setting device to {param}");

        if (Guid.TryParse(param, out var guid))
        {
            provider.SetAudioDevice(guid);
            return Ok();
        }

        logger.LogError($"Cannot set device to {param}");
        return Error("No such device");
    }

    public IActionResult GetVolume(string? _)
    {
        logger.LogInfo("Getting volume");

        return Text(provider.GetVolume());
    }

    public IActionResult SetVolume(string? param)
    {
        logger.LogInfo($"Setting volume to {param}");

        if (!int.TryParse(param, out var result))
        {
            logger.LogError($"Cannot set volume to {param}");
            return Error("Wrong volume format");
        }

        result = result > 100 ? 100 : result < 0 ? 0 : result;

        provider.SetVolume(result);

        return Text(result);
    }

    public IActionResult IncreaseBy5(string? _)
    {
        logger.LogInfo("Increasing volume by 5");

        var vol = provider.GetVolume();
        vol += 5;
        vol = vol > 100 ? 100 : vol < 0 ? 0 : vol;

        provider.SetVolume(vol);

        return Text(vol);
    }

    public IActionResult DecreaseBy5(string? _)
    {
        logger.LogInfo("Decreasing volume by 5");

        var vol = provider.GetVolume();
        vol -= 5;
        vol = vol > 100 ? 100 : vol < 0 ? 0 : vol;

        provider.SetVolume(vol);

        return Text(vol);
    }

    public IActionResult Mute(string? _)
    {
        logger.LogInfo("Toggling mute status");

        if (provider.IsMuted)
            provider.Unmute();
        else
            provider.Mute();

        return Ok();
    }
}