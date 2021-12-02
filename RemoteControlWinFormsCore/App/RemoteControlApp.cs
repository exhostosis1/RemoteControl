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

        public static void Start(Uri uri)
        {
            MyHttpListener.StartListen(uri);
        }

        public static void Stop()
        {
            MyHttpListener.StopListen();
        }

        public static void Restart(Uri url)
        {
            MyHttpListener.RestartListen(url);
        }

        public static void Restart()
        {
            MyHttpListener.RestartListen();
        }
    }
}