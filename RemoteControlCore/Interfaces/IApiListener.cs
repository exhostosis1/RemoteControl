using System;

namespace RemoteControlCore.Interfaces
{
    internal interface IApiListener: IListener
    {
        event ApiEventHandler OnApiRequest;

        void StartListen(UriBuilder ub);
        void RestartListen(UriBuilder ub);
    }
}
