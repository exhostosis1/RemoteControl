using RemoteControl.Core.Interfaces;
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

        public bool Listening { get; private set; }

        public void StartListen(string url)
        {
            if (_listener != null) return;

            _listener = new HttpListener();
            
            _listener.Prefixes.Add(url);

            _listener.Start();

            Task.Run(Start);

            Listening = true;
        }

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
                    StopListen();
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
    }
}
