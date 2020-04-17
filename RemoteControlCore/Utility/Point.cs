using System;
using System.Text.RegularExpressions;
using RemoteControlCore.Interfaces;

namespace RemoteControlCore.Utility
{
    internal class Point : ICoordinates
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point() { }

        public bool TrySetCoords(string input)
        {
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

            this.X = x;
            this.Y = y;

            return true;
        }
    }
}
