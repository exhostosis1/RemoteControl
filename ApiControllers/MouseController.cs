using Microsoft.Extensions.Logging;
using Shared;
using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;
using Shared.Enums;

namespace ApiControllers;

public class MouseController(IMouseControlProvider provider, ILogger logger) : BaseApiController
{
    public IActionResult Left(string? _)
    {
        logger.LogInformation("Pressing left mouse button");

        provider.MouseKeyPress();

        return Ok();
    }

    public IActionResult Right(string? _)
    {
        logger.LogInformation("Pressing right mouse button");

        provider.MouseKeyPress(MouseButtons.Right);

        return Ok();
    }

    public IActionResult Middle(string? _)
    {
        logger.LogInformation("Pressing middle mouse button");

        provider.MouseKeyPress(MouseButtons.Middle);

        return Ok();
    }

    public IActionResult WheelUp(string? _)
    {
        logger.LogInformation("Turning wheel up");

        provider.MouseWheel(true);

        return Ok();
    }

    public IActionResult WheelDown(string? _)
    {
        logger.LogInformation("Turning wheel down");

        provider.MouseWheel(false);

        return Ok();
    }

    public IActionResult DragStart(string? _)
    {
        logger.LogInformation("Starting drag");

        provider.MouseKeyPress(MouseButtons.Left, KeyPressMode.Down);
        Task.Run(async () =>
        {
            await Task.Delay(5_000);

            logger.LogInformation("Stopping drag");
            provider.MouseKeyPress(MouseButtons.Left, KeyPressMode.Up);
        });

        return Ok();
    }

    public IActionResult DragStop(string? _)
    {
        logger.LogInformation("Stopping drag");

        provider.MouseKeyPress(MouseButtons.Left, KeyPressMode.Up);

        return Ok();
    }

    public IActionResult Move(string? param)
    {
        if (string.IsNullOrWhiteSpace(param) || !Utils.TryGetCoords(param, out var x, out var y))
        {
            logger.LogError("Cannot move mouse by {param}", param);
            return Error("Wrong coordinates");
        }

        logger.LogInformation("Moving mouse by {param}", param);
        
        provider.MouseMove(x, y);

        return Ok();
    }
}