using Microsoft.Extensions.Logging;
using Servers.ApiControllers;
using Servers.DataObjects;
using Servers.DataObjects.Web;
using Servers.Results;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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

    public static ControllerActions GetControllersWithActions(this IEnumerable<IApiController> controllers)
    {
        return controllers.ToDictionary(
            x => x.GetType().Name.Replace("controller", "", StringComparison.OrdinalIgnoreCase).ToLower(),
            x => x.GetActions());
    }

    public static Dictionary<string, Delegate> GetActions(this IApiController controller)
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

public class ApiV1Middleware(IEnumerable<IApiController> controllers, ILogger logger) : IMiddleware
{
    private readonly ControllerActions _controllers = controllers.GetControllersWithActions();
    public static string ApiVersion => "v1";

    private static byte[] GetBytes(string? input) => Encoding.UTF8.GetBytes(input ?? string.Empty);
    
    public async Task ProcessRequestAsync(IContext contextParam, RequestDelegate next)
    {
        var context = (WebContext)contextParam;

        if (!ApiUtils.TryGetApiVersion(context.WebRequest.Path, out var version) || version != ApiVersion)
        {
            await next(contextParam);
            return;
        }

        logger.LogInformation("Processing api request {path}", context.WebRequest.Path);

        if (!ApiUtils.TryParsePath(context.WebRequest.Path, out var controllerName, out var actionName, out var param) 
            || !_controllers.TryGetValue(controllerName, out var controller) 
            || !controller.TryGetValue(actionName, out var action))
        {

            context.WebResponse.StatusCode = HttpStatusCode.NotFound;
            logger.LogError("Api method not found");
            return;
        }

        try
        {
            var result = action.DynamicInvoke(string.IsNullOrEmpty(param) ? [] : [param]) as IActionResult ?? throw new NullReferenceException("Action must return IActionResult");

            context.WebResponse.StatusCode = result.StatusCode;
            context.WebResponse.Payload = GetBytes(result.Result);

            if (result is JsonResult)
                context.WebResponse.ContentType = "application/json";
        }
        catch (Exception e)
        {
            logger.LogError("{message}", e.InnerException?.Message);

            context.WebResponse.StatusCode = HttpStatusCode.InternalServerError;
            context.WebResponse.Payload = Encoding.UTF8.GetBytes(e.InnerException?.Message ?? "");
        }
    }
}