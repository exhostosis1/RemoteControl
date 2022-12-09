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
        _input.KeyPress(KeysEnum.ArrowLeft);

        return Ok();
    }

    public IActionResult Forth(string? _)
    {
        _input.KeyPress(KeysEnum.ArrowRight);

        return Ok();
    }

    public IActionResult Pause(string? _)
    {
        _input.KeyPress(KeysEnum.MediaPlayPause);

        return Ok();
    }

    public IActionResult MediaBack(string? _)
    {
        _input.KeyPress(KeysEnum.MediaPrev);

        return Ok();
    }

    public IActionResult MediaForth(string? _)
    {
        _input.KeyPress(KeysEnum.MediaNext);

        return Ok();
    }

    public IActionResult MediaVolumeUp(string? _)
    {
        _input.KeyPress(KeysEnum.VolumeUp);

        return Ok();
    }

    public IActionResult MediaVolumeDown(string? _)
    {
        _input.KeyPress(KeysEnum.VolumeDown);

        return Ok();
    }

    public IActionResult MediaMute(string? _)
    {
        _input.KeyPress(KeysEnum.Mute);

        return Ok();
    }

    public IActionResult Text(string? param)
    {
        string text;

        try
        {
            text = WebUtility.UrlDecode(param) ?? throw new ArgumentNullException(nameof(param));
        }
        catch (Exception e)
        {
            return Error(e.Message);
        }

        _input.TextInput(text);
        _input.KeyPress(KeysEnum.Enter);

        return Ok();
    }
}