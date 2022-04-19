using RemoteControl.App.Web.Controllers;
using RemoteControl.App.Web.Listeners;

namespace RemoteControl.App
{
    public static class RemoteControlApp
    {
        private static readonly GenericListener UiListener = new();
        private static readonly GenericListener ApiListener = new();

        private const string ApiVersion = "v1";

        static RemoteControlApp()
        {
            UiListener.OnRequest += FileController.ProcessRequest;
            ApiListener.OnRequest += ApiControllerV1.ProcessRequest;
        }

        public static void Start(Uri uri)
        {
            UiListener.StartListen(uri.ToString());
            ApiListener.StartListen($"{uri}api/{ApiVersion}/");
        }

        public static void Stop()
        {
            UiListener.StopListen();
            ApiListener.StopListen();
        }

        public static string? GetUiListeningUri() => UiListener.ListeningUris.FirstOrDefault();
        public static string? GetApiListeningUri() => ApiListener.ListeningUris.FirstOrDefault();

        public static bool IsUiListening => UiListener.IsListening;
        public static bool IsApiListening => ApiListener.IsListening;
    }
}