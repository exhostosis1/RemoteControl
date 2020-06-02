using RemoteControlCore.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;
using MyLogger;

namespace RemoteControlCore.Listeners
{
    internal class MyHttpListenerRequestArgs : IHttpRequestArgs
    {
        public HttpListenerRequest Request { get; private set; }
        public HttpListenerResponse Response { get; private set; }

        public bool Simple { get; private set; }

        public MyHttpListenerRequestArgs(HttpListenerRequest request, HttpListenerResponse response, bool simple)
        {
            Request = request;
            Response = response;
            Simple = simple;
        }
    }
    internal class MyHttpListener : IHttpListener
    {
        private HttpListener _listener;
        private Logger log;

        public event HttpEventHandler OnApiRequest;
        public event HttpEventHandler OnHttpRequest;

        private string _url = "http://localhost/";
        private bool _simple;

        public MyHttpListener(string url) : this()
        {
            _url = url;
        }

        public MyHttpListener()
        {
           log = new Logger(this.GetType());
        }

        public void StartListen(string url, bool simple)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Url must be set before start listening");

            if (_listener != null) return;

            _url = url;
            _simple = simple;

            _listener = new HttpListener();

            _listener.Prefixes.Add(url);
                
            _listener.Start();

            Task.Run(Start);
        }

        public void StartListen()
        {
            StartListen(_url, _simple);
        }

        private Task Start()
        {
            while (true)
            {
                try
                {
                    ProcessRequest(_listener.GetContext());
                }
                catch
                {
                    return Task.CompletedTask;
                }
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            log.Log(context.Request.Url.LocalPath);

            var args = new MyHttpListenerRequestArgs(context.Request, context.Response, _simple);

            if (context.Request.Url.LocalPath.StartsWith("/api/"))
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
            }
        }

        public void RestartListen(string url, bool simple)
        {
            StopListen();
            StartListen(url, simple);
        }

        public void RestartListen()
        {
            RestartListen(_url, _simple);
        }
    }
}
