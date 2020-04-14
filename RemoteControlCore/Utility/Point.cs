using System;
using System.Text.RegularExpressions;

namespace RemoteControlCore.Utility
{
    internal class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static bool TryConvert(string input, out Point result)
        {
            result = null;

            var reg = new Regex("(?<=[xyXY]:[ ]*)[-0-9]+(?=[,} ])");
            int x;
            int y;

            try
            {
                var match = reg.Match(input);
                x = Convert.ToInt32(match.Value);
                y = Convert.ToInt32(match.NextMatch().Value);
            }
            catch
            {
                return false;
            }

            result = new Point(x, y);

            return true;
        }
    }
}
