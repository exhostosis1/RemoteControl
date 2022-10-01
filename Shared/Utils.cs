using Shared.Controllers;
using Shared.Controllers.Attributes;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Shared;

public static class Utils
{
    private static readonly Regex CoordRegex = new("[-0-9]+", RegexOptions.Compiled);

    public static bool TryGetCoords(string input, out int x, out int y)
    {
        x = 0;
        y = 0;

        var matches = CoordRegex.Matches(input);
        if (matches.Count < 2) return false;

        x = Convert.ToInt32(matches[0].Value);
        y = Convert.ToInt32(matches[1].Value);

        return true;
    }

    public static string? GetDisplayName<T>(this T type) where T : MemberInfo => 
        type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

    public static string? GetControllerName<T>(this T controller) where T : BaseController =>
        controller.GetType().GetCustomAttribute<ControllerAttribute>()?.Name;

    public static string? GetActionName<T>(this T type) where T : MethodInfo =>
        type.GetCustomAttribute<ActionAttribute>()?.Name;

    public static bool TryParsePath(this string path, string apiVersion, out string controller, out string action, out string? parameter)
    {
        var split = path[(path.IndexOf(apiVersion, StringComparison.Ordinal) + apiVersion.Length)..]
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        controller = string.Empty;
        action = string.Empty;
        parameter = null;

        if (split.Length < 2)
            return false;

        controller = split[0];
        action = split[1];
            
        if(split.Length > 2)
            parameter = split[2];

        return true;
    }

    public static bool TryParseConfig(this string config, out string parameter, out string value)
    {
        var split = config.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        parameter = string.Empty;
        value = string.Empty;

        if (split.Length < 2)
            return false;

        parameter = split[0];
        value = split[1];

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
}