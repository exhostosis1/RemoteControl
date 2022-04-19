using System.Text.RegularExpressions;

namespace RemoteControl.App.Utility
{
    internal static class Utils
    {
        //(?<=[xy]:[ ]*) match starts with 'x' or 'y' then ':' and zero or more ' '
        //[-0-9]+ main pattern. one or more '-' or numbers
        //(?=[,} ]) match ends with ',', '}', ' ', or ']'
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

        /// <summary>
        /// parse last 2 params in address string. e.g. "http://host/api/method/methodParam 
        /// </summary>

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
