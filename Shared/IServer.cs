using System;

namespace Shared
{
    public interface IServer
    {
        public void Start(Uri uri);
        public void Stop();
        public string? GetListeningUri();
        public bool IsListening { get; }
    }
}