using Shared.Controllers.Results;
using Shared.Logging.Interfaces;
using System;
using System.Reflection;

namespace Shared.Controllers;

public abstract class BaseController
{
    protected readonly ILogger Logger;

    protected BaseController(ILogger logger)
    {
        Logger = logger;
    }

    protected static IActionResult Ok() => new OkResult();
    protected static IActionResult Error(string? message) => new ErrorResult(message);
    protected static IActionResult Json(object data) => new JsonResult(data);
    protected static IActionResult Text(object data) => new StringResult(data);

    public ControllerMethods GetMethods()
    {
        var values = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);

        var result = new ControllerMethods();

        foreach (var method in values)
        {
            var parameters = method.GetParameters();

            if (method.ReturnType != typeof(IActionResult) || parameters.Length != 1 ||
                parameters[0].ParameterType != typeof(string))
                continue;

            result.Add(method.Name.ToLower(), method.CreateDelegate<Func<string?, IActionResult>>(this));
        }

        return result;
    }
}