using RemoteControl.Core.Interfaces;
using System.Net;

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
        private readonly HttpListener _listener = new HttpListener();

        public void RestartListen()
        {
            StopListen();
            StartListen();
        }

        public event HttpEventHandler? OnHttpRequest;
        public event HttpEventHandler? OnApiRequest;

        public void StartListen(string url)
        {
            if (_listener.IsListening) return;

            if (!string.IsNullOrEmpty(url))
            {
                _listener.Prefixes.Clear();
                _listener.Prefixes.Add(url);
            }

            if (_listener.Prefixes.Count == 0) return;

            _listener.Start();

            Task.Factory.StartNew(Start, TaskCreationOptions.LongRunning);
        }

        public void StartListen() => StartListen(string.Empty);

        private void Start()
        {
            while (true)
            {
                try
                {
                    ProcessRequest(_listener.GetContext());
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

            if (context?.Request?.RawUrl?.Contains("api") ?? false)
            {
                OnApiRequest?.Invoke(args);
            }
            else
            {
                OnHttpRequest?.Invoke(args);
            }

            context?.Response.Close();
        }

        public void StopListen()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
            }
        }

        public void RestartListen(string url)
        {
            StopListen();
            StartListen(url);
        }
    }
}
