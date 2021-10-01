using System.Collections.Generic;

namespace RemoteControl.Core.Interfaces
{
    internal interface IListener
    {
        void StartListen(string url);
        void StopListen();
        void RestartListen(string url);

        event HttpEventHandler OnHttpRequest;
        event HttpEventHandler OnApiRequest;

        bool Listening { get; }
    }
}
