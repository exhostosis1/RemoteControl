using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;
using System.Net;

namespace ApiControllers;

public class KeyboardController: BaseApiController
{
    private readonly IKeyboardControlProvider _input;
    private readonly ILogger<KeyboardController> _logger;

    public KeyboardController(IKeyboardControlProvider input, ILogger<KeyboardController> logger) : base(logger)
    {
        _logger = logger;
        _input = input;
    }

    public IActionResult Back(string? _)
    {
        _logger.LogInfo("Pressing arrow left");

        _input.KeyPress(KeysEnum.ArrowLeft);

        return Ok();
    }

    public IActionResult Forth(string? _)
    {
        _logger.LogInfo("Pressing arrow right");

        _input.KeyPress(KeysEnum.ArrowRight);

        return Ok();
    }

    public IActionResult Pause(string? _)
    {
        _logger.LogInfo("Pressing pause");

        _input.KeyPress(KeysEnum.MediaPlayPause);

        return Ok();
    }

    public IActionResult MediaBack(string? _)
    {
        _logger.LogInfo("Pressing previous");

        _input.KeyPress(KeysEnum.MediaPrev);

        return Ok();
    }

    public IActionResult MediaForth(string? _)
    {
        _logger.LogInfo("Pressing next");

        _input.KeyPress(KeysEnum.MediaNext);

        return Ok();
    }

    public IActionResult MediaVolumeUp(string? _)
    {
        _logger.LogInfo("Pressing volume up");

        _input.KeyPress(KeysEnum.VolumeUp);

        return Ok();
    }

    public IActionResult MediaVolumeDown(string? _)
    {
        _logger.LogInfo("Pressing volume down");

        _input.KeyPress(KeysEnum.VolumeDown);

        return Ok();
    }

    public IActionResult MediaMute(string? _)
    {
        _logger.LogInfo("Pressing mute");

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
            _logger.LogError($"Cannot decode text {param}");
            return Error(e.Message);
        }

        _logger.LogInfo($"Inputing text {text}");

        _input.TextInput(text);
        _input.KeyPress(KeysEnum.Enter);

        return Ok();
    }
}