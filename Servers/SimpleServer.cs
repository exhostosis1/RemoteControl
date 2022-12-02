using Shared.Config;
using Shared.Server;
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

        public void Start(AppConfig config) => _listener.StartListen(config.ServerConfig.Uri);
        public void Stop() => _listener.StopListen();
        public Uri? GetListeningUri() => _listener.ListeningUris.FirstOrDefault();
        public bool IsListening => _listener.IsListening;
    }
}