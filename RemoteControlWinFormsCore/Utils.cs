using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

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

        public static void Deconstruct(this string[] list, out string lastButOne, out string last)
        {
            if (list.Length < 2)
            {
                lastButOne = last = string.Empty;
                return;
            }

            lastButOne = list[^2];
            last = list[^1];
        }

        internal static string? GetDisplayName<T>(this T type) where T: MemberInfo
        {
            return type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
        }
    }
}
