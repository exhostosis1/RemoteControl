namespace RemoteControl.Core.Interfaces
{
    internal interface ICoordinates
    {
        int X { get; }
        int Y { get; }

        bool TrySetCoords(string input);
    }
}
