using Shared.ApiControllers;
using Shared.ApiControllers.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Shared;

public static partial class Utils
{
    private const string ApiPath = "/api/";
    private const string ControllerGroupName = "controller";
    private const string ActionGroupName = "action";
    private const string ParamGroupName = "param";

    [GeneratedRegex("[0-9A-F]{8}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{4}[-][0-9A-F]{12}", RegexOptions.IgnoreCase)]
    public static partial Regex GuidRegex();

    [GeneratedRegex("[-0-9]+")]
    public static partial Regex CoordRegex();

    [GeneratedRegex(
        $"(?<={ApiPath}v\\d+)\\/(?<{ControllerGroupName}>[a-z]+)\\/(?<{ActionGroupName}>[a-z]+)\\/?(?<{ParamGroupName}>.*?)(?=\\/|$)",
        RegexOptions.IgnoreCase)]
    public static partial Regex ApiRegex();

    [GeneratedRegex($"(?<={ApiPath})v\\d+", RegexOptions.IgnoreCase)]
    public static partial Regex ApiVersionRegex();

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

    public static void RunWindowsCommand(string command, out string result, out string error) =>
        RunCommand(command, OSPlatform.Windows, out result!, out error!, false);

    public static void RunWindowsCommandAsAdmin(string command) =>
        RunCommand(command, OSPlatform.Windows, out _, out _, true);

    public static void RunLinuxCommand(string command, out string result, out string error) =>
        RunCommand(command, OSPlatform.Linux, out result!, out error!, false);

    public static void RunCommand(string command, OSPlatform platform, out string? result, out string? error,
        bool elevated)
    {
        if (platform != OSPlatform.Windows && platform != OSPlatform.Linux)
            throw new Exception("OS not supported");

        var proc = new Process();

        proc.StartInfo.FileName = platform == OSPlatform.Windows ? "cmd" : "/bin/bash";
        proc.StartInfo.Arguments = $"{(platform == OSPlatform.Windows ? "/" : "-")}c \"{command}\"";
        proc.StartInfo.CreateNoWindow = true;

        if (!elevated)
        {
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            if (platform == OSPlatform.Windows)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                proc.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
                proc.StartInfo.StandardErrorEncoding = Encoding.GetEncoding(866);
            }
        }
        else
        {
            proc.StartInfo.Verb = "runas";
            proc.StartInfo.UseShellExecute = true;
        }

        proc.Start();

        result = elevated ? null : proc.StandardOutput.ReadToEnd();
        error = elevated ? null : proc.StandardError.ReadToEnd();

        proc.WaitForExit();
    }

    public static IEnumerable<string> GetCurrentIPs() =>
        Dns.GetHostAddresses(Dns.GetHostName(), AddressFamily.InterNetwork).Select(x => x.ToString());

    public static ControllersWithMethods GetControllersWithMethods(this IEnumerable<IApiController> controllers)
    {
        return new ControllersWithMethods(controllers.ToDictionary(
            x => x.GetType().Name.Replace("controller", "", StringComparison.OrdinalIgnoreCase).ToLower(),
            x => x.GetMethods()));
    }

    public static ControllerMethods GetMethods(this IApiController controller)
    {
        return new ControllerMethods(controller.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(x =>
        {
            var parameters = x.GetParameters();
            return x.ReturnType == typeof(IActionResult) && parameters.Length == 1 &&
                   parameters[0].ParameterType == typeof(string);
        }).ToDictionary(x => x.Name.ToLower(), x => x.CreateDelegate<Func<string?, IActionResult>>(controller)));
    }

    public static void AddFirewallRule(Uri uri)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new Exception("OS not supported");

        var command =
            $"netsh advfirewall firewall add rule name=\"Remote Control\" dir=in action=allow profile=private localip={uri.Host} localport={uri.Port} protocol=tcp";

        RunWindowsCommandAsAdmin(command);
    }

    public static void AddFirewallRule(IEnumerable<Uri> uris)
    {
        foreach (var uri in uris)
        {
            AddFirewallRule(uri);
        }
    }

    public static async Task<T> PostAsJsonAndGetResultAsync<T>(this HttpClient client, string? uri, object content,
        JsonSerializerOptions? options = null, CancellationToken token = default)
    {
        HttpResponseMessage response;

        try
        {
            response = await client.PostAsJsonAsync(uri, content, options, token);
        }
        catch (TaskCanceledException e)
        {
            if (e.InnerException is TimeoutException)
                throw e.InnerException;

            throw;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Error sending request", null, response.StatusCode);
        }

        return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(token)) ?? throw new JsonException("Cannot parse response");
    }
}