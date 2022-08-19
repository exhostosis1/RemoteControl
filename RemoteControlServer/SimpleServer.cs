using Shared;
using Shared.Server.Interfaces;

namespace Server
{
    public class SimpleServer: IRemoteControlApp
    {
        private readonly IListener _listener;

        public SimpleServer(IListener listener, IMiddleware middleware)
        {
            _listener = listener;
            _listener.OnRequest += middleware.ProcessRequest;
        }

        public void Start(Uri uri) => _listener.StartListen(uri.ToString());

        public void Stop() => _listener.StopListen();

        public string? GetUiListeningUri() => _listener.ListeningUris.FirstOrDefault();
        public string? GetApiListeningUri() => _listener.ListeningUris.FirstOrDefault();

        public bool IsUiListening => _listener.IsListening;
        public bool IsApiListening => _listener.IsListening;
    }
}