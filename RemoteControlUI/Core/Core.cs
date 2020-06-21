using System;
using RemoteControl.Core.Abstract;
using RemoteControl.Core.Controllers;
using RemoteControl.Core.Interfaces;
using RemoteControl.Core.Listeners;

namespace RemoteControl.Core
{
    public class Main
    {
        private readonly IListener _httplistener;
        private UriBuilder HttpHost { get; }

        public Main(UriBuilder http)
        {
            HttpHost = http;

            AbstractController apiController = new ApiController();
            AbstractController httpController = new HttpController();

            _httplistener = new MyHttpListener();
            _httplistener.OnHttpRequest += httpController.ProcessRequest;
            _httplistener.OnApiRequest += apiController.ProcessRequest;
        }

        public void Start()
        {
            _httplistener.StartListen(HttpHost);
        }

        public void Stop()
        {
            _httplistener.StopListen();
        }

        public void Restart()
        {
            _httplistener.RestartListen(HttpHost);
        } 
    }
}