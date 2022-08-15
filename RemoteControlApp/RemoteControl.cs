using Shared.Interfaces.Web;

namespace RemoteControlApp
{
    public class RemoteControl
    {
        private readonly IListener _uiListener;
        private readonly IListener _apiListener;

        private const string ApiVersion = "v1";

        public RemoteControl(IListener uiListener, IListener apiListener, IMiddleware uiMiddlewareChain, IMiddleware apiMiddlewareChain)
        {
            _apiListener = apiListener;
            _uiListener = uiListener;

            _uiListener.OnRequest += uiMiddlewareChain.ProcessRequest;
            _apiListener.OnRequest += apiMiddlewareChain.ProcessRequest;
        }

        public void Start(Uri uri)
        {
            _uiListener.StartListen(uri.ToString());
            _apiListener.StartListen($"{uri}api/{ApiVersion}/");
        }

        public void Stop()
        {
            _uiListener.StopListen();
            _apiListener.StopListen();
        }

        public string? GetUiListeningUri() => _uiListener.ListeningUris.FirstOrDefault();
        public string? GetApiListeningUri() => _apiListener.ListeningUris.FirstOrDefault();

        public bool IsUiListening => _uiListener.IsListening;
        public bool IsApiListening => _apiListener.IsListening;
    }
}