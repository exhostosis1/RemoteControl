using System.Collections.Generic;

namespace RemoteControl.Core.Interfaces
{
    internal interface IListener
    {
        void StartListen(IReadOnlyCollection<string> urls);
        void StopListen();
        void RestartListen(IReadOnlyCollection<string> urls);
        void StartListen();
        void RestartListen();

        event HttpEventHandler OnHttpRequest;
        event HttpEventHandler OnApiRequest;

        bool Listening { get; }
    }
}
