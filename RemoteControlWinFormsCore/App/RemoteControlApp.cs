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

        public static void Start(params Uri[] uris) => MyHttpListener.StartListen(uris);

        public static void Stop() => MyHttpListener.StopListen();

        public static void Restart(params Uri[] uris) => MyHttpListener.RestartListen(uris);

        public static IEnumerable<string> GetCurrentIps() => MyHttpListener.ListeningUris;

        public static bool IsListening() => MyHttpListener.IsListening;
    }
}