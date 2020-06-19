using System;

namespace RemoteControlCore.Interfaces
{
    internal interface IListener
    {
        void StartListen(UriBuilder ub);
        void StopListen();
        void RestartListen(UriBuilder ub);

        bool Listening { get; }
    }
}
