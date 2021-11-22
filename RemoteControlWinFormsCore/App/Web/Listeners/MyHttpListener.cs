using RemoteControl.App.Web.Interfaces;
using System.Net;

namespace RemoteControl.App.Web.Listeners
{
    internal class MyHttpListener : IListener
    {
        private readonly HttpListener _listener = new();

        public void RestartListen()
        {
            StopListen();
            StartListen();
        }

        public event HttpEventHandler? OnRequest;

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

            Task.Factory.StartNew(Listen, TaskCreationOptions.LongRunning);
        }

        public void StartListen() => StartListen(string.Empty);

        private void Listen()
        {
            while (true)
            {
                HttpListenerContext context;

                try
                {
                    context = _listener.GetContext();
                }
                catch { return; }

                context.Response.StatusCode = 200;

                OnRequest?.Invoke(context);

                context?.Response.Close();
            }
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
