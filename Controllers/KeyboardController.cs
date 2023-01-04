using Shared.Controllers;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;
using System.Net;
using Shared.Controllers.Results;

namespace Controllers;

public class KeyboardController: BaseController
{
    private readonly IKeyboardControlProvider _input;

    public KeyboardController(IKeyboardControlProvider input, ILogger logger) : base(logger)
    {
        _input = input;
    }

    public IActionResult Back(string? _)
    {
        Logger.LogInfo("Pressing arrow left");

        _input.KeyPress(KeysEnum.ArrowLeft);

        return Ok();
    }

    public IActionResult Forth(string? _)
    {
        Logger.LogInfo("Pressing arrow right");

        _input.KeyPress(KeysEnum.ArrowRight);

        return Ok();
    }

    public IActionResult Pause(string? _)
    {
        Logger.LogInfo("Pressing pause");

        _input.KeyPress(KeysEnum.MediaPlayPause);

        return Ok();
    }

    public IActionResult MediaBack(string? _)
    {
        Logger.LogInfo("Pressing previous");

        _input.KeyPress(KeysEnum.MediaPrev);

        return Ok();
    }

    public IActionResult MediaForth(string? _)
    {
        Logger.LogInfo("Pressing next");

        _input.KeyPress(KeysEnum.MediaNext);

        return Ok();
    }

    public IActionResult MediaVolumeUp(string? _)
    {
        Logger.LogInfo("Pressing volume up");

        _input.KeyPress(KeysEnum.VolumeUp);

        return Ok();
    }

    public IActionResult MediaVolumeDown(string? _)
    {
        Logger.LogInfo("Pressing volume down");

        _input.KeyPress(KeysEnum.VolumeDown);

        return Ok();
    }

    public IActionResult MediaMute(string? _)
    {
        Logger.LogInfo("Pressing mute");

        _input.KeyPress(KeysEnum.Mute);

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
            Logger.LogError($"Cannot decode text {param}");
            return Error(e.Message);
        }

        Logger.LogInfo($"Inputing text {text}");

        _input.TextInput(text);
        _input.KeyPress(KeysEnum.Enter);

        return Ok();
    }
}