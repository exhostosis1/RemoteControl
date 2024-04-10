using Microsoft.Extensions.Logging;
using Servers.ApiControllers;
using Servers.Results;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Servers.DataObjects;
using ControllerActions = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, System.Delegate>>;

[assembly: InternalsVisibleTo("UnitTests")]

namespace Servers.Middleware;

internal static partial class ApiUtils
{
    private const string ApiPath = "/api/";
    private const string ControllerGroupName = "controller";
    private const string ActionGroupName = "action";
    private const string ParamGroupName = "param";

    [GeneratedRegex(
        $@"(?<={ApiPath}v\d+)\/(?<{ControllerGroupName}>[a-z]+)\/(?<{ActionGroupName}>[a-z]+)\/?(?<{ParamGroupName}>.*?)(?=\/|$)",
        RegexOptions.IgnoreCase)]
    public static partial Regex ApiRegex();

    [GeneratedRegex($"(?<={ApiPath})v\\d+", RegexOptions.IgnoreCase)]
    public static partial Regex ApiVersionRegex();

    public static bool TryGetApiVersion(string path, out string? version)
    {
        version = null;
        var match = ApiVersionRegex().Match(path);

        if (!match.Success) return false;

        version = match.Value;
        return true;
    }

    public static bool TryParsePath(string path, out string controller, out string action, out string parameter)
    {
        var match = ApiRegex().Match(path);

        controller = string.Empty;
        action = string.Empty;
        parameter = string.Empty;

        if (!match.Success) return false;

        controller = match.Groups[ControllerGroupName].Value.ToLower();
        action = match.Groups[ActionGroupName].Value.ToLower();
        parameter = match.Groups[ParamGroupName].Value;

        return true;
    }

    public static ControllerActions GetControllersWithActions(this IEnumerable<BaseApiController> controllers)
    {
        return controllers.ToDictionary(
            x => x.GetType().Name.Replace("controller", "", StringComparison.OrdinalIgnoreCase).ToLower(),
            x => x.GetActions());
    }

    public static Dictionary<string, Delegate> GetActions(this BaseApiController controller)
    {
        var methods = controller.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.ReturnType == typeof(IActionResult));

        return methods.ToDictionary(method => method.Name.ToLower(), method =>
        {
            var paramsExpr = method.GetParameters().Select(x => Expression.Parameter(x.ParameterType, x.Name))
                .ToArray();

            var contExpr = Expression.Constant(controller);
            var callExpr = Expression.Call(contExpr, method, paramsExpr as Expression[]);

            return Expression.Lambda(callExpr, paramsExpr).Compile();
        });
    }
}

public class ApiV1Middleware(IEnumerable<BaseApiController> controllers, ILogger logger) : IMiddleware
{
    private readonly ControllerActions _controllers = controllers.GetControllersWithActions();
    public static string ApiVersion => "v1";

    private static byte[] GetBytes(string? input) => Encoding.UTF8.GetBytes(input ?? string.Empty);
    
    public async Task ProcessRequestAsync(RequestContext context, RequestDelegate next)
    {
        if (!ApiUtils.TryGetApiVersion(context.Input.Path, out var version) || version != ApiVersion)
        {
            await next(context);
            return;
        }

        logger.LogInformation("Processing api request {path}", context.Input.Path);

        if (!ApiUtils.TryParsePath(context.Input.Path, out var controllerName, out var actionName, out var param) 
            || !_controllers.TryGetValue(controllerName, out var controller) 
            || !controller.TryGetValue(actionName, out var action))
        {

            context.Output.StatusCode = HttpStatusCode.NotFound;
            logger.LogError("Api method not found");
            return;
        }

        try
        {
            var result = action.DynamicInvoke(string.IsNullOrEmpty(param) ? [] : [param]) as IActionResult ?? throw new NullReferenceException("Action must return IActionResult");

            context.Output.StatusCode = result.StatusCode;
            context.Output.Payload = GetBytes(result.Result);

            if (result is JsonResult)
                context.Output.ContentType = "application/json";
        }
        catch (Exception e)
        {
            logger.LogError("{message}", e.InnerException?.Message);

            context.Output.StatusCode = HttpStatusCode.InternalServerError;
            context.Output.Payload = Encoding.UTF8.GetBytes(e.InnerException?.Message ?? "");
        }
    }
}