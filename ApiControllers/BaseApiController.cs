using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using Shared.Logging.Interfaces;

namespace ApiControllers;

public abstract class BaseApiController : IApiController
{
    private readonly ILogger _logger;

    protected BaseApiController(ILogger logger)
    {
        _logger = logger;
    }

    protected static IActionResult Ok() => new OkResult();
    protected static IActionResult Error(string? message) => new ErrorResult(message);
    protected static IActionResult Json(object data) => new JsonResult(data);
    protected static IActionResult Text(object data) => new TextResult(data);
}