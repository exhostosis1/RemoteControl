namespace RemoteControlCore.Interfaces
{
    internal interface ICoordinates
    {
        int X { get; set; }
        int Y { get; set; }

        bool TrySetCoords(string input);
    }
}
