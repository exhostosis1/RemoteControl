using Shared;
using Shared.Controllers;
using Shared.Controllers.Attributes;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Controllers;

[Controller(MethodNames.MouseControllerName)]
public class MouseController : BaseController
{
    private readonly IMouseControlProvider _input;

    public MouseController(IMouseControlProvider input, ILogger logger) : base(logger)
    {
        _input = input;
    }

    [Action(MethodNames.MouseLeftButton)]
    public string? Left(string? _)
    {
        _input.ButtonPress();

        return "done";
    }

    [Action(MethodNames.MouseRightButton)]
    public string? Right(string? _)
    {
        _input.ButtonPress(MouseButtons.Right);

        return "done";
    }

    [Action(MethodNames.MouseMiddleButton)]
    public string? Middle(string? _)
    {
        _input.ButtonPress(MouseButtons.Middle);

        return "done";
    }

    [Action(MethodNames.MouseWheelUp)]
    public string? DragUp(string? _)
    {
        _input.Wheel(true);

        return "done";
    }

    [Action(MethodNames.MouseWheelDown)]
    public string? DragDown(string? _)
    {
        _input.Wheel(false);

        return "done";
    }

    [Action(MethodNames.MouseDragStart)]
    public string? DragStart(string? _)
    {
        _input.ButtonPress(MouseButtons.Left, KeyPressMode.Down);
        Task.Run(async () =>
        {
            await Task.Delay(5_000);
            _input.ButtonPress(MouseButtons.Left, KeyPressMode.Up);
        });

        return "done";
    }

    [Action(MethodNames.MouseDragStop)]
    public string? DragStop(string? _)
    {
        _input.ButtonPress(MouseButtons.Left, KeyPressMode.Up);

        return "done";
    }

    [Action(MethodNames.MouseMove)]
    public string? Move(string? param)
    {
        if (Utils.TryGetCoords(param ?? "", out var x, out var y))
        {
            _input.Move(x, y);
        }

        return null;
    }
}