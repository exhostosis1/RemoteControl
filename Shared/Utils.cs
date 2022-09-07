using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        public static string? GetDisplayName<T>(this T type) where T: MemberInfo
        {
            return type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
        }

        public static (string, string, string) ParsePath(this string path, string apiVersion)
        {
            var split = path[(path.IndexOf(apiVersion, StringComparison.Ordinal) + apiVersion.Length)..]
                .Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (split.Length < 2)
            {
                return ("", "", "");
            }
            else if (split.Length < 3)
            {
                return (split[0], split[1], "");
            }
            else
            {
                return (split[0], split[1], split[2]);
            }
        }

        public static (string, string) ParseConfig(this string config)
        {
            var split = config.Split('=').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            return split.Length < 2 ? ("", "") : (split[0], split[1]);
        }

        public static void RunLinuxCommand(string command, out string result, out string error)
        {
            using var proc = new Process();

            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.Arguments = "-c \" " + command + " \"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;

            proc.Start();

            result = proc.StandardOutput.ReadToEnd();
            error = proc.StandardError.ReadToEnd();

            proc.WaitForExit();
        }

        public static T CreateDelegate<T>(this MethodInfo methodInfo, object target) where T: Delegate
        {
            return (T)methodInfo.CreateDelegate(typeof(T), target);
        }
    }
}
