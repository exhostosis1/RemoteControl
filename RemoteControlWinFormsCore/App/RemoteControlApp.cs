using RemoteControl.App.Web.Controllers;
using RemoteControl.App.Web.Listeners;

namespace RemoteControl.App
{
    public static class RemoteControlApp
    {
        static RemoteControlApp()
        {
            MyHttpListener.OnRequest += BaseController.ProcessRequest;    
        }

        public static void Start(Uri uri)
        {
            MyHttpListener.StartListen(uri.ToString());
        }

        public static void Stop()
        {
            MyHttpListener.StopListen();
        }

        public static void Restart(Uri url)
        {
            MyHttpListener.RestartListen(url.ToString());
        }

        public static void Restart()
        {
            MyHttpListener.RestartListen();
        }
    }
}