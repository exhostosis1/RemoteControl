namespace RemoteControlCore.Interfaces
{
    internal interface IHttpListener: IListener
    {
        void RestartListen(string url, bool simple);
        void StartListen(string url, bool simple);

        event HttpEventHandler OnHttpRequest;
    }
}
