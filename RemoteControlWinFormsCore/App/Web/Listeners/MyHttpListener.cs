using RemoteControl.App.Web.Interfaces;
using System.Net;

namespace RemoteControl.App.Web.Listeners
{
    internal static class MyHttpListener
    {
        private static readonly ILogger _logger = Logger.GetFileLogger(typeof(MyHttpListener));

        private static readonly HttpListener _listener = new();
        public static bool IsListening => _listener.IsListening;
        public static IEnumerable<string> ListeningUris 
        { 
            get 
            { 
                foreach (var p in _listener.Prefixes) 
                    yield return p; 
            } 
        }

        public static void RestartListen()
        {
            StopListen();
            StartListen();
        }

        public static event HttpEventHandler? OnRequest;

        public static void StartListen(Uri url) => StartListen(new[] { url });

        public static void StartListen(IEnumerable<Uri> urls)
        {
            if (_listener.IsListening) StopListen();

            foreach (var url in urls)
            {
                _listener.Prefixes.Add(url.ToString());
            }

            if (_listener.Prefixes.Count == 0) return;

            _listener.Start();

            Task.Factory.StartNew(Listen, TaskCreationOptions.LongRunning);
        }

        public static void StartListen() => StartListen(Enumerable.Empty<Uri>());

        private static void Listen()
        {
            while (true)
            {
                HttpListenerContext context;

                try
                {
                    context = _listener.GetContext();
                }
                catch(Exception e) 
                {
                    _listener.Stop();
                    _logger.Log(ErrorLevel.Error, e.Message);
                    return; 
                }

                OnRequest?.Invoke(context);

                context?.Response.Close();
            }
        }

        public static void StopListen()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
            }
        }

        public static void RestartListen(Uri url)
        {
            StopListen();
            StartListen(url);
        }

        public static void RestartListen(IEnumerable<Uri> uris)
        {
            StopListen();
            StartListen(uris);
        }
    }
}
