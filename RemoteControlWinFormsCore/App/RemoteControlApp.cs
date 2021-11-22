using RemoteControl.App.Web.Controllers;
using RemoteControl.App.Web.Interfaces;
using RemoteControl.App.Web.Listeners;

namespace RemoteControl.App
{
    public class RemoteControlApp
    {
        private readonly IListener _httplistener;

        public RemoteControlApp()
        {
            _httplistener = new MyHttpListener();
            _httplistener.OnRequest += BaseController.ProcessRequest;
        }

        public void Start(Uri uri)
        {
            _httplistener.StartListen(uri.ToString());
        }

        public void Stop()
        {
            _httplistener.StopListen();
        }

        public void Restart(Uri url)
        {
            _httplistener.RestartListen(url.ToString());
        }

        public void Restart()
        {
            _httplistener.RestartListen();
        }
    }
}