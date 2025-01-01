using MainApp.ControlProviders.Enums;
using MainApp.ControlProviders.Interfaces;
using MainApp.Servers.Results;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace MainApp.Servers.ApiControllers;

internal static partial class CoordsHelper
{
    [GeneratedRegex("[-0-9]+")]
    public static partial Regex CoordRegex();

    public static bool TryGetCoords(string input, out int x, out int y)
    {
        x = 0;
        y = 0;

        var matches = CoordRegex().Matches(input);
        if (matches.Count < 2) return false;

        x = Convert.ToInt32(matches[0].Value);
        y = Convert.ToInt32(matches[1].Value);

        return true;
    }
}

internal class MouseController(IMouseControl provider, ILogger logger) : BaseApiController
{
    public IActionResult Left()
    {
        logger.LogInformation("Pressing left mouse button");

        provider.MouseKeyPress();

        return Ok();
    }

    public IActionResult Right()
    {
        logger.LogInformation("Pressing right mouse button");

        provider.MouseKeyPress(MouseButtons.Right);

        return Ok();
    }

    public IActionResult Middle()
    {
        logger.LogInformation("Pressing middle mouse button");

        provider.MouseKeyPress(MouseButtons.Middle);

        return Ok();
    }

    public IActionResult WheelUp()
    {
        logger.LogInformation("Turning wheel up");

        provider.MouseWheel(true);

        return Ok();
    }

    public IActionResult WheelDown()
    {
        logger.LogInformation("Turning wheel down");

        provider.MouseWheel(false);

        return Ok();
    }

    public IActionResult DragStart()
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

    public IActionResult DragStop()
    {
        logger.LogInformation("Stopping drag");

        provider.MouseKeyPress(MouseButtons.Left, KeyPressMode.Up);

        return Ok();
    }

    public IActionResult Move(string param)
    {
        if (string.IsNullOrWhiteSpace(param) || !CoordsHelper.TryGetCoords(param, out var x, out var y))
        {
            logger.LogError("Cannot move mouse by {param}", param);
            return Error("Wrong coordinates");
        }

        logger.LogInformation("Moving mouse by {param}", param);

        provider.MouseMove(x, y);

        return Ok();
    }
}