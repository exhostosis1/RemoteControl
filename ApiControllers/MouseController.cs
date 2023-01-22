using Shared;
using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;
using Shared.Enums;
using Shared.Logging.Interfaces;

namespace ApiControllers;

public class MouseController : BaseApiController
{
    private readonly IMouseControlProvider _provider;
    private readonly ILogger<MouseController> _logger;

    public MouseController(IMouseControlProvider provider, ILogger<MouseController> logger) : base(logger)
    {
        _logger = logger;
        _provider = provider;
    }

    public IActionResult Left(string? _)
    {
        _logger.LogInfo("Pressing left mouse button");

        _provider.MouseKeyPress();

        return Ok();
    }

    public IActionResult Right(string? _)
    {
        _logger.LogInfo("Pressing right mouse button");

        _provider.MouseKeyPress(MouseButtons.Right);

        return Ok();
    }

    public IActionResult Middle(string? _)
    {
        _logger.LogInfo("Pressing middle mouse button");

        _provider.MouseKeyPress(MouseButtons.Middle);

        return Ok();
    }

    public IActionResult WheelUp(string? _)
    {
        _logger.LogInfo("Turning wheel up");

        _provider.MouseWheel(true);

        return Ok();
    }

    public IActionResult WheelDown(string? _)
    {
        _logger.LogInfo("Turning wheel down");

        _provider.MouseWheel(false);

        return Ok();
    }

    public IActionResult DragStart(string? _)
    {
        _logger.LogInfo("Starting drag");

        _provider.MouseKeyPress(MouseButtons.Left, KeyPressMode.Down);
        Task.Run(async () =>
        {
            await Task.Delay(5_000);

            _logger.LogInfo("Stopping drag");
            _provider.MouseKeyPress(MouseButtons.Left, KeyPressMode.Up);
        });

        return Ok();
    }

    public IActionResult DragStop(string? _)
    {
        _logger.LogInfo("Stopping drag");

        _provider.MouseKeyPress(MouseButtons.Left, KeyPressMode.Up);

        return Ok();
    }

    public IActionResult Move(string? param)
    {
        if (string.IsNullOrWhiteSpace(param))
        {
            _logger.LogError($"Cannot move mouse by {param}");
            return Error("Empty coordinates");
        }

        _logger.LogInfo($"Moving mouse by {param}");

        if (Utils.TryGetCoords(param, out var x, out var y))
        {
            _provider.MouseMove(x, y);
        }

        return Ok();
    }
}