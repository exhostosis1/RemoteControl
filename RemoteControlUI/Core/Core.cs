using System;
using RemoteControl.Core.Controllers;
using RemoteControl.Core.Interfaces;
using RemoteControl.Core.Listeners;

namespace RemoteControl.Core
{
    public class Main
    {
        private readonly IListener _httplistener;

        public Main()
        {
            _httplistener = new MyHttpListener();
            _httplistener.OnHttpRequest += new HttpController().ProcessRequest;
            _httplistener.OnApiRequest += new ApiController().ProcessRequest;
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
            Stop();
            Start(url);
        }
    }
}