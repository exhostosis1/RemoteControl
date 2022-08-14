using System.Text.RegularExpressions;

namespace RemoteControl.App.Utility
{
    internal static class Utils
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
        
        public static void Deconstruct(this string[] list, out string first, out string second)
        {
            if(list.Length < 2)
            {
                first = second = string.Empty;
                return;
            }

            first = list[^2];
            second = list[^1];
        }
    }
}
