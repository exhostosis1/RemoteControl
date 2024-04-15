using ControlProviders.Interfaces;
using Microsoft.Extensions.Logging;
using Servers.Results;

namespace Servers.ApiControllers;

public class AudioController(IAudioControl provider, ILogger logger) : BaseApiController
{
    public IActionResult GetDevices()
    {
        logger.LogInformation("Getting devices");

        return Json(provider.GetAudioDevices());
    }

    public IActionResult SetDevice(string param)
    {
        logger.LogInformation("Setting device to {param}", param);

        if (Guid.TryParse(param, out var guid))
        {
            provider.SetAudioDevice(guid);
            return Ok();
        }

        logger.LogError("Cannot set device to {param}", param);
        return Error("No such device");
    }

    public IActionResult GetVolume()
    {
        logger.LogInformation("Getting volume");

        return Text(provider.GetVolume());
    }

    public IActionResult SetVolume(string param)
    {
        logger.LogInformation("Setting volume to {param}", param);

        if (!int.TryParse(param, out var result))
        {
            logger.LogError("Cannot set volume to {param}", param);
            return Error("Wrong volume format");
        }

        result = result > 100 ? 100 : result < 0 ? 0 : result;

        provider.SetVolume(result);

        return Text(result);
    }

    public IActionResult IncreaseByFive()
    {
        logger.LogInformation("Increasing volume by 5");

        var vol = provider.GetVolume();
        vol += 5;
        vol = vol > 100 ? 100 : vol < 0 ? 0 : vol;

        provider.SetVolume(vol);

        return Text(vol);
    }

    public IActionResult DecreaseByFive()
    {
        logger.LogInformation("Decreasing volume by 5");

        var vol = provider.GetVolume();
        vol -= 5;
        vol = vol > 100 ? 100 : vol < 0 ? 0 : vol;

        provider.SetVolume(vol);

        return Text(vol);
    }

    public IActionResult Mute()
    {
        logger.LogInformation("Toggling mute status");

        if (provider.IsMuted)
            provider.Unmute();
        else
            provider.Mute();

        return Ok();
    }
}