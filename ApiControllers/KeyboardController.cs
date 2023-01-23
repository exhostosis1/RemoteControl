using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;
using Shared.Enums;
using Shared.Logging.Interfaces;
using System.Net;

namespace ApiControllers;

public class KeyboardController : BaseApiController
{
    private readonly IKeyboardControlProvider _provider;
    private readonly ILogger<KeyboardController> _logger;

    public KeyboardController(IKeyboardControlProvider provider, ILogger<KeyboardController> logger) : base(logger)
    {
        _logger = logger;
        _provider = provider;
    }

    public IActionResult Back(string? _)
    {
        _logger.LogInfo("Pressing arrow left");

        _provider.KeyboardKeyPress(KeysEnum.ArrowLeft);

        return Ok();
    }

    public IActionResult Forth(string? _)
    {
        _logger.LogInfo("Pressing arrow right");

        _provider.KeyboardKeyPress(KeysEnum.ArrowRight);

        return Ok();
    }

    public IActionResult Pause(string? _)
    {
        _logger.LogInfo("Pressing pause");

        _provider.KeyboardKeyPress(KeysEnum.MediaPlayPause);

        return Ok();
    }

    public IActionResult MediaBack(string? _)
    {
        _logger.LogInfo("Pressing previous");

        _provider.KeyboardKeyPress(KeysEnum.MediaPrev);

        return Ok();
    }

    public IActionResult MediaForth(string? _)
    {
        _logger.LogInfo("Pressing next");

        _provider.KeyboardKeyPress(KeysEnum.MediaNext);

        return Ok();
    }

    public IActionResult MediaVolumeUp(string? _)
    {
        _logger.LogInfo("Pressing volume up");

        _provider.KeyboardKeyPress(KeysEnum.VolumeUp);

        return Ok();
    }

    public IActionResult MediaVolumeDown(string? _)
    {
        _logger.LogInfo("Pressing volume down");

        _provider.KeyboardKeyPress(KeysEnum.VolumeDown);

        return Ok();
    }

    public IActionResult MediaMute(string? _)
    {
        _logger.LogInfo("Pressing mute");

        _provider.KeyboardKeyPress(KeysEnum.Mute);

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

        _provider.TextInput(text);
        _provider.KeyboardKeyPress(KeysEnum.Enter);

        return Ok();
    }
}