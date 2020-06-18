using RemoteControlCore.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;
using MyLogger;

namespace RemoteControlCore.Listeners
{
    internal class MyHttpListenerRequestArgs : IHttpRequestArgs
    {
        public HttpListenerRequest Request { get; }
        public HttpListenerResponse Response { get; }

        public bool Simple { get; }

        public MyHttpListenerRequestArgs(HttpListenerRequest request, HttpListenerResponse response, bool simple)
        {
            Request = request;
            Response = response;
            Simple = simple;
        }
    }
    internal class MyHttpListener : IHttpListener, IApiListener
    {
        private HttpListener _listener;
        private readonly ILogger _logger;

        public event ApiEventHandler OnApiRequest;

        public event HttpEventHandler OnHttpRequest;

        private string _url = "http://localhost/";
        private bool _simple;

        public bool Listening { get; private set; }

        public MyHttpListener(string url) : this()
        {
            _url = url;
        }

        public MyHttpListener()
        {
            _logger = Logger.GetFileLogger("log.txt", this.GetType());
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

            Listening = true;
        }

        public void StartListen()
        {
            StartListen(_url, _simple);
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
            _logger.Log(context.Request.RawUrl);

            if (context.Request.RawUrl.StartsWith("/api/"))
            {
                OnApiRequest?.Invoke(context.Request.RawUrl, context.Response.OutputStream);
            }
            else
            {
                OnHttpRequest?.Invoke(new MyHttpListenerRequestArgs(context.Request, context.Response, _simple));
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

        public void RestartListen(string url, bool simple)
        {
            StopListen();
            StartListen(url, simple);
        }

        public void RestartListen()
        {
            RestartListen(_url, _simple);
        }

        public void StartListen(UriBuilder ub)
        {
            StartListen(ub.Uri.ToString(), false);
        }

        public void RestartListen(UriBuilder ub)
        {
            RestartListen(ub.Uri.ToString(), false);
        }
    }
}
