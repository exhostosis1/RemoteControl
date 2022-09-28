using Shared.Controllers;
using Shared.Controllers.Attributes;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Shared
{
    public static class Utils
    {
        private static readonly Regex CoordRegex = new Regex("[-0-9]+", RegexOptions.Compiled);

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
            var split = config.Split('=').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            parameter = string.Empty;
            value = string.Empty;

            if (split.Length < 2)
                return false;

            parameter = split[0];
            value = split[1];

            return true;
        }

        public static void RunWindowsCommand(string command, out string result, out string error)
        {
            using var proc = new Process();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            proc.StartInfo.FileName = "cmd";
            proc.StartInfo.Arguments = $"/c \"{command}\"";
            proc.StartInfo.CreateNoWindow = true;
            
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
            proc.StartInfo.StandardErrorEncoding = Encoding.GetEncoding(866);
            proc.StartInfo.UseShellExecute = false;

            proc.Start();

            result = proc.StandardOutput.ReadToEnd();
            error = proc.StandardError.ReadToEnd();

            proc.WaitForExit();
        }

        public static void RunWindowsCommandAsAdmin(string command)
        {
            using var proc = new Process();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            proc.StartInfo.FileName = "cmd";
            proc.StartInfo.Arguments = $"/c \"{command}\"";
            proc.StartInfo.CreateNoWindow = true;

            proc.StartInfo.Verb = "runas";
            proc.StartInfo.UseShellExecute = true;

            proc.Start();

            proc.WaitForExit();
        }

        public static void RunLinuxCommand(string command, out string result, out string error)
        {
            using var proc = new Process();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.Arguments = $"-c \"{command}\"";
            proc.StartInfo.CreateNoWindow = true;

            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();

            result = proc.StandardOutput.ReadToEnd();
            error = proc.StandardError.ReadToEnd();

            proc.WaitForExit();
        }

        public static T CreateDelegate<T>(this MethodInfo methodInfo, object target) where T: Delegate => (T)methodInfo.CreateDelegate(typeof(T), target);
    }
}
