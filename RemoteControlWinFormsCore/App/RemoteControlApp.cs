using RemoteControl.App.Web.Controllers;
using RemoteControl.App.Web.Listeners;

namespace RemoteControl.App
{
    public static class RemoteControlApp
    {
        static RemoteControlApp()
        {
            MyHttpListener.OnRequest += Router.ProcessRequest;
        }

        public static void Start(params Uri[] uris)
        {
            MyHttpListener.StartListen(uris);
        }

        public static void Stop()
        {
            MyHttpListener.StopListen();
        }

        public static void Restart() => MyHttpListener.StopListen();

        public static IEnumerable<string> GetCurrentUris => MyHttpListener.ListeningUris;

        public static bool IsListening => MyHttpListener.IsListening;
    }
}