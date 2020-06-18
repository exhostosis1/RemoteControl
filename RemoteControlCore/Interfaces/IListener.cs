namespace RemoteControlCore.Interfaces
{
    internal interface IListener
    {
        void StartListen();
        void StopListen();
        void RestartListen();

        bool Listening { get; }
    }
}
