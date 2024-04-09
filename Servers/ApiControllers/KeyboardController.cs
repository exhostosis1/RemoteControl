using System.Net;
using Microsoft.Extensions.Logging;
using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;
using Shared.Enums;

namespace Servers.ApiControllers;

public class KeyboardController(IKeyboardControlProvider provider, ILogger logger) : BaseApiController
{
    public IActionResult Back()
    {
        logger.LogInformation("Pressing arrow left");

        provider.KeyboardKeyPress(KeysEnum.ArrowLeft);

        return Ok();
    }

    public IActionResult Forth()
    {
        logger.LogInformation("Pressing arrow right");

        provider.KeyboardKeyPress(KeysEnum.ArrowRight);

        return Ok();
    }

    public IActionResult Pause()
    {
        logger.LogInformation("Pressing pause");

        provider.KeyboardKeyPress(KeysEnum.MediaPlayPause);

        return Ok();
    }

    public IActionResult MediaBack()
    {
        logger.LogInformation("Pressing previous");

        provider.KeyboardKeyPress(KeysEnum.MediaPrev);

        return Ok();
    }

    public IActionResult MediaForth()
    {
        logger.LogInformation("Pressing next");

        provider.KeyboardKeyPress(KeysEnum.MediaNext);

        return Ok();
    }

    public IActionResult MediaVolumeUp()
    {
        logger.LogInformation("Pressing volume up");

        provider.KeyboardKeyPress(KeysEnum.VolumeUp);

        return Ok();
    }

    public IActionResult MediaVolumeDown()
    {
        logger.LogInformation("Pressing volume down");

        provider.KeyboardKeyPress(KeysEnum.VolumeDown);

        return Ok();
    }

    public IActionResult MediaMute()
    {
        logger.LogInformation("Pressing mute");

        provider.KeyboardKeyPress(KeysEnum.Mute);

        return Ok();
    }

    public IActionResult BrowserBack()
    {
        logger.LogInformation("Pressing browser back");

        provider.KeyboardKeyPress(KeysEnum.BrowserBack);

        return Ok();
    }

    public IActionResult BrowserForward()
    {
        logger.LogInformation("Pressing browser forward");

        provider.KeyboardKeyPress(KeysEnum.BrowserForward);

        return Ok();
    }

    public IActionResult Text(string param)
    {
        string text;

        try
        {
            text = WebUtility.UrlDecode(param) ?? throw new ArgumentException("Cannot decode text", nameof(param));
        }
        catch (Exception e)
        {
            logger.LogError("Cannot decode text {param}", param);
            return Error(e.Message);
        }

        logger.LogInformation("Inputting text {text}", text);

        provider.TextInput(text);
        provider.KeyboardKeyPress(KeysEnum.Enter);

        return Ok();
    }
}