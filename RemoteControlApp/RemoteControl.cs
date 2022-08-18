using Shared.Interfaces;
using Shared.Interfaces.Web;

namespace RemoteControlApp
{
    public class RemoteControl: IRemoteControlApp
    {
        private readonly IListener _listener;

        public RemoteControl(IListener listener, HttpEventHandler handler)
        {
            _listener = listener;
            _listener.OnRequest += handler;
        }

        public void Start(Uri uri) => _listener.StartListen(uri.ToString());

        public void Stop() => _listener.StopListen();

        public string? GetUiListeningUri() => _listener.ListeningUris.FirstOrDefault();
        public string? GetApiListeningUri() => _listener.ListeningUris.FirstOrDefault();

        public bool IsUiListening => _listener.IsListening;
        public bool IsApiListening => _listener.IsListening;
    }
}