using Shared.Logging.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using Shared.ApiControllers.Results;

namespace Shared.ApiControllers;

public abstract class BaseApiController: IApiController
{
    private readonly ILogger _logger;

    protected BaseApiController(ILogger logger)
    {
        _logger = logger;
    }

    protected static IActionResult Ok() => new OkResult();
    protected static IActionResult Error(string? message) => new ErrorResult(message);
    protected static IActionResult Json(object data) => new JsonResult(data);
    protected static IActionResult Text(object data) => new StringResult(data);

    public ControllerMethods GetMethods()
    {
        return new ControllerMethods(GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(x =>
        {
            var parameters = x.GetParameters();
            return x.ReturnType == typeof(IActionResult) && parameters.Length == 1 &&
                   parameters[0].ParameterType == typeof(string);
        }).ToDictionary(x => x.Name.ToLower(), x => x.CreateDelegate<Func<string?, IActionResult>>(this)));
    }
}