using System;

namespace RemoteControlCore.Interfaces
{
    internal interface IHttpListener
    {
        event HttpEventHandler OnApiRequest;
        event HttpEventHandler OnHttpRequest;
        void StartListen();
        void StartListen(string url);
        void StopListen();
        void RestartListen(string url);
        void RestartListen();
    }

    internal delegate void HttpEventHandler(IHttpRequestArgs args);
}
