using RemoteControlCore.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RemoteControlCore.Listeners
{
    internal class MyHttpListenerRequestArgs : IHttpRequestArgs
    {
        public HttpListenerRequest Request { get; private set; }
        public HttpListenerResponse Response { get; private set; }

        public MyHttpListenerRequestArgs(HttpListenerRequest request, HttpListenerResponse response)
        {
            Request = request;
            Response = response;
        }
    }
    internal class MyHttpListener : IHttpListener
    {
        private HttpListener _listener;

        public event HttpEventHandler OnApiRequest;
        public event HttpEventHandler OnHttpRequest;

        private string _url = "http://localhost/";

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

            try
            {
                _listener.Prefixes.Add(url);
                
                _listener.Start();
            }
            catch { throw; }

            Task.Run(Start);
        }

        public void StartListen()
        {
            StartListen(_url);
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
            var args = new MyHttpListenerRequestArgs(context.Request, context.Response);

            if (context.Request.Url.LocalPath == "/api" && context.Request.QueryString.Count > 0)
            {
                OnApiRequest(args);
            }
            else
            {
                OnHttpRequest(args);
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

        public void RestartListen(string url)
        {
            StopListen();
            StartListen(url);
        }

        public void RestartListen()
        {
            RestartListen(_url);
        }
    }
}
