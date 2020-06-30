using System;
using System.Net;
using System.Threading.Tasks;
using RemoteControl.Core.Interfaces;

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

        private string _url = "http://localhost/";

        public bool Listening { get; private set; }

        public MyHttpListener(string url)
        {
            _url = url;
        }

        public MyHttpListener()
        {
            
        }

        public void StartListen(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Url must be set before start listening");

            if (_listener != null) return;

            _url = url;

            _listener = new HttpListener();

            _listener.Prefixes.Add(url);
                
            _listener.Start();

            Task.Run(Start);

            Listening = true;
        }

        public void StartListen()
        {
            StartListen(_url);
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

        public void RestartListen(string url)
        {
            StopListen();
            StartListen(url);
        }

        public void RestartListen()
        {
            RestartListen(_url);
        }

        public void StartListen(UriBuilder ub)
        {
            StartListen(ub.Uri.ToString());
        }

        public void RestartListen(UriBuilder ub)
        {
            RestartListen(ub.Uri.ToString());
        }
    }
}
