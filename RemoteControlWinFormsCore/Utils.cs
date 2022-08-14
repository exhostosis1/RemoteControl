using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using RemoteControl.App.Web.Attributes;

namespace RemoteControl
{
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

        internal static string? GetDisplayName<T>(this T type) where T: MemberInfo
        {
            return type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
        }

        internal static string? GetControllerName(this Type controller)
        {
            return controller.GetCustomAttribute<ControllerAttribute>()?.Name;
        }

        internal static string? GetActionName(this MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<ActionAttribute>()?.Name;
        }

        internal static (string, string, string) ParsePath(this string path, string apiVersion)
        {
            var split = path[(path.IndexOf(apiVersion, StringComparison.Ordinal) + apiVersion.Length)..]
                .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return split.Length switch
            {
                < 2 => ("", "", ""),
                < 3 => (split[0], split[1], ""),
                _ => (split[0], split[1], split[2])
            };
        }

        internal static (string, string) ParseConfig(this string config)
        {
            var split = config.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return split.Length < 2 ? ("", "") : (split[0], split[1]);
        }
    }
}
