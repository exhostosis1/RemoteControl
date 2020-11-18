namespace RemoteControl.Core.Interfaces
{
    internal interface IListener
    {
        void StartListen(string[] urls);
        void StopListen();
        void RestartListen(string[] urls);

        event HttpEventHandler OnHttpRequest;
        event HttpEventHandler OnApiRequest;

        bool Listening { get; }
    }
}
