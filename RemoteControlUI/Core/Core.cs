using RemoteControl.Core.Abstract;
using RemoteControl.Core.Controllers;
using RemoteControl.Core.Interfaces;
using RemoteControl.Core.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteControl.Core
{
    public class Main
    {
        private readonly IListener _httplistener;

        public Main()
        {
            AbstractController apiController = new ApiController();
            AbstractController httpController = new HttpController();

            _httplistener = new MyHttpListener();
            _httplistener.OnHttpRequest += httpController.ProcessRequest;
            _httplistener.OnApiRequest += apiController.ProcessRequest;
        }

        public void Start(string scheme, IEnumerable<string> hosts, int port)
        {
            string[] uris;
            uris = hosts.Select(x => new UriBuilder(scheme, x, port).ToString()).ToArray();

            _httplistener.StartListen(uris);
        }

        public void Stop()
        {
            _httplistener.StopListen();
        }
    }
}