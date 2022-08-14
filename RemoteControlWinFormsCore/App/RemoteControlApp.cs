using RemoteControl.App.Interfaces.Web;

namespace RemoteControl.App
{
    public class RemoteControlApp
    {
        private readonly IListener _uiListener;
        private readonly IListener _apiListener;

        private const string ApiVersion = "v1";

        public RemoteControlApp(IListener uiListener, IListener apiListener, IMiddleware fileController, IMiddleware apiController)
        {
            _apiListener = apiListener;
            _uiListener = uiListener;

            _uiListener.OnRequest += fileController.ProcessRequest;
            _apiListener.OnRequest += apiController.ProcessRequest;
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