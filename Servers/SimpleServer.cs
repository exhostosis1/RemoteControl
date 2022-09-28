using Shared;
using Shared.Server.Interfaces;

namespace Servers
{
    public class SimpleServer: IServer
    {
        private readonly IListener _listener;

        public SimpleServer(IListener listener, IMiddleware middleware)
        {
            _listener = listener;
            _listener.OnRequest += middleware.ProcessRequest;
        }

        public void Start(Uri uri) => _listener.StartListen(uri);
        public void Stop() => _listener.StopListen();
        public string? GetListeningUri() => _listener.ListeningUris.FirstOrDefault();
        public bool IsListening => _listener.IsListening;
    }
}