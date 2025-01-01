using MainApp.Servers.ApiControllers;
using MainApp.Servers.DataObjects;
using MainApp.Servers.Results;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using ControllerActions = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, System.Delegate>>;

namespace MainApp.Servers.Middleware;

internal static partial class ApiUtils
{
    private const string ApiPath = "/api/";
    private const string ControllerGroupName = "controller";
    private const string ActionGroupName = "action";
    private const string ParamGroupName = "param";

    [GeneratedRegex(
        $@"(?<={ApiPath}v\d+)\/(?<{ControllerGroupName}>[a-z]+)\/(?<{ActionGroupName}>[a-z]+)\/?(?<{ParamGroupName}>.*?)(?=\/|$)",
        RegexOptions.IgnoreCase)]
    private static partial Regex ApiRegex();

    [GeneratedRegex($"(?<={ApiPath})v\\d+", RegexOptions.IgnoreCase)]
    private static partial Regex ApiVersionRegex();

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

    private static Dictionary<string, Delegate> GetActions(this BaseApiController controller)
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

internal class ApiV1Middleware(IEnumerable<BaseApiController> controllers, ILogger logger) : IMiddleware
{
    private readonly ControllerActions _controllers = controllers.GetControllersWithActions();
    private static string ApiVersion => "v1";

    public async Task ProcessRequestAsync(RequestContext context, RequestDelegate next)
    {
        if (!ApiUtils.TryGetApiVersion(context.Request, out var version) || version != ApiVersion)
        {
            await next(context);
            return;
        }

        logger.LogInformation("Processing api request {path}", context.Request);

        if (!ApiUtils.TryParsePath(context.Request, out var controllerName, out var actionName, out var param)
            || !_controllers.TryGetValue(controllerName, out var controller)
            || !controller.TryGetValue(actionName, out var action))
        {

            context.Status = RequestStatus.NotFound;
            logger.LogError("Api method not found");
            return;
        }

        try
        {
            var result = action.DynamicInvoke(string.IsNullOrEmpty(param) ? [] : [param]) as IActionResult ?? throw new NullReferenceException("Action must return IActionResult");

            context.Status = result.StatusCode;
            context.Reply = result.Result ?? "";
        }
        catch (Exception e)
        {
            logger.LogError("{message}", e.InnerException?.Message);

            context.Status = RequestStatus.Error;
            context.Reply = e.InnerException?.Message ?? "";
        }
    }
}