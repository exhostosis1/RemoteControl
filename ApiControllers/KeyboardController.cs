using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;
using Shared.Enums;
using Shared.Logging.Interfaces;
using System.Net;

namespace ApiControllers;

public class KeyboardController(IKeyboardControlProvider provider, ILogger<KeyboardController> logger) : BaseApiController
{
    public IActionResult Back(string? _)
    {
        logger.LogInfo("Pressing arrow left");

        provider.KeyboardKeyPress(KeysEnum.ArrowLeft);

        return Ok();
    }

    public IActionResult Forth(string? _)
    {
        logger.LogInfo("Pressing arrow right");

        provider.KeyboardKeyPress(KeysEnum.ArrowRight);

        return Ok();
    }

    public IActionResult Pause(string? _)
    {
        logger.LogInfo("Pressing pause");

        provider.KeyboardKeyPress(KeysEnum.MediaPlayPause);

        return Ok();
    }

    public IActionResult MediaBack(string? _)
    {
        logger.LogInfo("Pressing previous");

        provider.KeyboardKeyPress(KeysEnum.MediaPrev);

        return Ok();
    }

    public IActionResult MediaForth(string? _)
    {
        logger.LogInfo("Pressing next");

        provider.KeyboardKeyPress(KeysEnum.MediaNext);

        return Ok();
    }

    public IActionResult MediaVolumeUp(string? _)
    {
        logger.LogInfo("Pressing volume up");

        provider.KeyboardKeyPress(KeysEnum.VolumeUp);

        return Ok();
    }

    public IActionResult MediaVolumeDown(string? _)
    {
        logger.LogInfo("Pressing volume down");

        provider.KeyboardKeyPress(KeysEnum.VolumeDown);

        return Ok();
    }

    public IActionResult MediaMute(string? _)
    {
        logger.LogInfo("Pressing mute");

        provider.KeyboardKeyPress(KeysEnum.Mute);

        return Ok();
    }

    public IActionResult BrowserBack(string? _)
    {
        logger.LogInfo("Pressing browser back");

        provider.KeyboardKeyPress(KeysEnum.BrowserBack);

        return Ok();
    }

    public IActionResult BrowserForward(string? _)
    {
        logger.LogInfo("Pressing browser forward");

        provider.KeyboardKeyPress(KeysEnum.BrowserForward);

        return Ok();
    }

    public IActionResult Text(string? param)
    {
        string text;

        try
        {
            text = WebUtility.UrlDecode(param) ?? throw new ArgumentException(nameof(param));
        }
        catch (Exception e)
        {
            logger.LogError($"Cannot decode text {param}");
            return Error(e.Message);
        }

        logger.LogInfo($"Inputing text {text}");

        provider.TextInput(text);
        provider.KeyboardKeyPress(KeysEnum.Enter);

        return Ok();
    }
}