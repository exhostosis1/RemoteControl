using RemoteControl.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemoteControl.Core.Listeners
{
    internal class MyHttpListenerRequestArgs : IHttpRequestArgs
    {
        public HttpListenerRequest Request { get; }
        public HttpListenerResponse Response { get; }

        public MyHttpListenerRequestArgs(HttpListenerRequest request, HttpListenerResponse response)
        {
            Request = request;
            Response = response;
        }
    }
    internal class MyHttpListener : IListener
    {
        private HttpListener _listener;

        public event HttpEventHandler OnHttpRequest;
        public event HttpEventHandler OnApiRequest;

        private string[] _urls = { "http://localhost/" };

        public bool Listening { get; private set; }

        public MyHttpListener(string[] urls)
        {
            _urls = urls;
        }

        public MyHttpListener()
        {
            
        }

        public void StartListen(IReadOnlyCollection<string> urls)
        {
            if (urls == null || urls.Count == 0)
                throw new ArgumentException("Url must be set before start listening");

            if (_listener != null) return;

            _urls = urls.ToArray();

            _listener = new HttpListener();

            foreach (var url in _urls)
            {
                _listener.Prefixes.Add(url);
            }

            _listener.Start();

            Task.Run(Start);

            Listening = true;
        }

        public void StartListen()
        {
            StartListen(_urls);
        }

        private async void Start()
        {
            while (true)
            {
                try
                {
                    ProcessRequest(await _listener.GetContextAsync());
                }
                catch
                {
                    return;
                }
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            context.Response.StatusCode = 200;

            var args = new MyHttpListenerRequestArgs(context.Request, context.Response);

            if (context.Request.RawUrl.Contains("api"))
            {
                OnApiRequest?.Invoke(args);
            }
            else
            {
                OnHttpRequest?.Invoke(args);
            }

            context.Response.Close();
        }

        public void StopListen()
        {
            if (_listener != null)
            {
                _listener.Abort();
                _listener.Close();
                _listener = null;
                Listening = false;
            }
        }

        public void RestartListen(IReadOnlyCollection<string> urls)
        {
            StopListen();
            StartListen(urls);
        }

        public void RestartListen()
        {
            RestartListen(_urls);
        }
    }
}
