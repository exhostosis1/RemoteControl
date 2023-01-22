using Shared.ApiControllers.Results;
using Shared.ControlProviders.Provider;
using Shared.Logging.Interfaces;

namespace ApiControllers;

public class DisplayController: BaseApiController
{
    private readonly IDisplayControlProvider _provider;
    private readonly ILogger<DisplayController> _logger;

    public DisplayController(IDisplayControlProvider provider, ILogger<DisplayController> logger) : base(logger)
    {
        _logger = logger;
        _provider = provider;
    }

    public IActionResult Darken(string? _)
    {
        _logger.LogInfo("Turning off display");

        _provider.DisplayOff();

        return Ok();
    }
}