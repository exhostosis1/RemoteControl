using System;

namespace RemoteControl.Core.Interfaces
{
    internal interface IListener
    {
        void StartListen(UriBuilder ub);
        void StopListen();
        void RestartListen(UriBuilder ub);

        event HttpEventHandler OnHttpRequest;
        event HttpEventHandler OnApiRequest;

        bool Listening { get; }
    }
}
