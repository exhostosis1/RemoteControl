using System.Collections.Generic;

namespace Shared.Server.Interfaces
{
    public interface IListener
    {
        public bool IsListening { get; }
        public IReadOnlyCollection<string> ListeningUris { get; }

        public event HttpEventHandler? OnRequest;

        public void StartListen(string url);

        public void StopListen();
    }
}
