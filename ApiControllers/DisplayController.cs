using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;
using Shared.Logging.Interfaces;

namespace ApiControllers;

public class DisplayController(IDisplayControlProvider provider, ILogger<DisplayController> logger) : BaseApiController
{
    private readonly IDisplayControlProvider _provider = provider;
    private readonly ILogger<DisplayController> _logger = logger;

    public IActionResult Darken(string? _)
    {
        _logger.LogInfo("Turning off display");

        _provider.DisplayOff();

        return Ok();
    }
}