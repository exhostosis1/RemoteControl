using RemoteControlCore.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using MyLogger;

namespace RemoteControlCore.Listeners
{
    internal class MyHttpApiListener : IApiListener
    {
        private HttpListener _listener;
        private readonly ILogger _logger;

        public event ApiEventHandler OnApiRequest;

        private string _url = "http://localhost/";

        public bool Listening { get; private set; }

        public MyHttpApiListener(string url) : this()
        {
            _url = url;
        }

        public MyHttpApiListener()
        {
            _logger = Logger.GetFileLogger("log.txt", this.GetType());
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
            _logger.Log(context.Request.RawUrl);

            context.Response.StatusCode = 200;
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");

            var result = OnApiRequest?.Invoke(context.Request.RawUrl);

            if (!string.IsNullOrEmpty(result))
            {
                using (var sw = new StreamWriter(context.Response.OutputStream))
                {
                    sw.Write(result);
                }
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
