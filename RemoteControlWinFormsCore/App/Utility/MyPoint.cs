using System.Text.RegularExpressions;

namespace RemoteControl.App.Utility
{
    internal class MyPoint
    {
        private readonly Regex CoordRegex = new("(?<=[xyXY]:[ ] *)[-0-9]+(?=[,} ])");

        public int X { get; private set; }
        public int Y { get; private set; }

        public MyPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public MyPoint() { }

        public bool TrySetCoords(string input)
        {
            int x;
            int y;
                
            try
            {
                var match = CoordRegex.Match(input);
                x = Convert.ToInt32(match.Value);
                y = Convert.ToInt32(match.NextMatch().Value);
            }
            catch
            {
                return false;
            }

            X = x;
            Y = y;

            return true;
        }
    }
}
