using Shared;
using Shared.ApiControllers;
using Shared.Controllers;
using Shared.Controllers.Results;
using Shared.ControlProviders;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace ApiControllers;

public class MouseController : BaseApiController
{
    private readonly IMouseControlProvider _input;

    public MouseController(IMouseControlProvider input, ILogger logger) : base(logger)
    {
        _input = input;
    }

    public IActionResult Left(string? _)
    {
        Logger.LogInfo("Pressing left mouse button");

        _input.ButtonPress();

        return Ok();
    }

    public IActionResult Right(string? _)
    {
        Logger.LogInfo("Pressing right mouse button");

        _input.ButtonPress(MouseButtons.Right);

        return Ok();
    }

    public IActionResult Middle(string? _)
    {
        Logger.LogInfo("Pressing middle mouse button");

        _input.ButtonPress(MouseButtons.Middle);

        return Ok();
    }

    public IActionResult WheelUp(string? _)
    {
        Logger.LogInfo("Turning wheel up");

        _input.Wheel(true);

        return Ok();
    }

    public IActionResult WheelDown(string? _)
    {
        Logger.LogInfo("Turning wheel down");

        _input.Wheel(false);

        return Ok();
    }

    public IActionResult DragStart(string? _)
    {
        Logger.LogInfo("Starting drag");

        _input.ButtonPress(MouseButtons.Left, KeyPressMode.Down);
        Task.Run(async () =>
        {
            await Task.Delay(5_000);

            Logger.LogInfo("Stopping drag");
            _input.ButtonPress(MouseButtons.Left, KeyPressMode.Up);
        });

        return Ok();
    }

    public IActionResult DragStop(string? _)
    {
        Logger.LogInfo("Stopping drag");

        _input.ButtonPress(MouseButtons.Left, KeyPressMode.Up);

        return Ok();
    }

    public IActionResult Move(string? param)
    {
        if (string.IsNullOrWhiteSpace(param))
        {
            Logger.LogError($"Cannot move mouse by {param}");
            return Error("Empty coordinates");
        }

        Logger.LogInfo($"Moving mouse by {param}");

        if (Utils.TryGetCoords(param, out var x, out var y))
        {
            _input.Move(x, y);
        }

        return Ok();
    }
}