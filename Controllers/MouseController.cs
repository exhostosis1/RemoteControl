using Shared;
using Shared.Controllers;
using Shared.Controllers.Results;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace Controllers;

public class MouseController : BaseController
{
    private readonly IMouseControlProvider _input;

    public MouseController(IMouseControlProvider input, ILogger logger) : base(logger)
    {
        _input = input;
    }

    public IActionResult Left(string? _)
    {
        _input.ButtonPress();

        return Ok();
    }

    public IActionResult Right(string? _)
    {
        _input.ButtonPress(MouseButtons.Right);

        return Ok();
    }

    public IActionResult Middle(string? _)
    {
        _input.ButtonPress(MouseButtons.Middle);

        return Ok();
    }

    public IActionResult WheelUp(string? _)
    {
        _input.Wheel(true);

        return Ok();
    }

    public IActionResult WheelDown(string? _)
    {
        _input.Wheel(false);

        return Ok();
    }

    public IActionResult DragStart(string? _)
    {
        _input.ButtonPress(MouseButtons.Left, KeyPressMode.Down);
        Task.Run(async () =>
        {
            await Task.Delay(5_000);
            _input.ButtonPress(MouseButtons.Left, KeyPressMode.Up);
        });

        return Ok();
    }

    public IActionResult DragStop(string? _)
    {
        _input.ButtonPress(MouseButtons.Left, KeyPressMode.Up);

        return Ok();
    }

    public IActionResult Move(string? param)
    {
        if (param == null)
            return Error("Empty coordinates");

        if (Utils.TryGetCoords(param, out var x, out var y))
        {
            _input.Move(x, y);
        }

        return Ok();
    }
}