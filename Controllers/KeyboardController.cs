using System.Net;
using Shared.Controllers;
using Shared.Controllers.Attributes;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Controllers;

[Controller(MethodNames.KeyboardControllerName)]
public class KeyboardController: BaseController
{
    private readonly IKeyboardControlProvider _input;

    public KeyboardController(IKeyboardControlProvider input, ILogger logger) : base(logger)
    {
        _input = input;
    }

    [Action(MethodNames.KeyboardArrowBack)]
    public string? Back(string? _)
    {
        _input.KeyPress(KeysEnum.ArrowLeft);

        return "done";
    }

    [Action(MethodNames.KeyboardArrowForth)]
    public string? Forth(string? _)
    {
        _input.KeyPress(KeysEnum.ArrowRight);

        return "done";
    }

    [Action(MethodNames.KeyboardMediaPause)]
    public string? Pause(string? _)
    {
        _input.KeyPress(KeysEnum.MediaPlayPause);

        return "done";
    }

    [Action(MethodNames.KeyboardMediaBack)]
    public string? MediaBack(string? _)
    {
        _input.KeyPress(KeysEnum.MediaPrev);

        return "done";
    }

    [Action(MethodNames.KeyboardMediaForth)]
    public string? MediaForth(string? _)
    {
        _input.KeyPress(KeysEnum.MediaNext);

        return "done";
    }

    [Action(MethodNames.KeyboardText)]
    public string? TextInput(string? param)
    {
        _input.TextInput(WebUtility.UrlDecode(param ?? ""));
        _input.KeyPress(KeysEnum.Enter);

        return "done";
    }
}