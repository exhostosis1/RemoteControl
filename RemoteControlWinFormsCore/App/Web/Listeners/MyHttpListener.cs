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

        public static event HttpEventHandler? OnRequest;

        public static async void StartListen(params Uri[] urls)
        {
            if (_listener.IsListening) StopListen();

            _listener.Prefixes.Clear();

            foreach (var url in urls)
            {
                _listener.Prefixes.Add(url.ToString());
            }

            if (_listener.Prefixes.Count == 0) return;

            _listener.Start();

            while (true)
            {
                try
                {
                    var context = await _listener.GetContextAsync();

                    OnRequest?.Invoke(context);
                    context?.Response.Close();
                }
                catch
                {
                    if (_listener.IsListening)
                        continue;
                    else
                        return;
                }
            }
        }                

        public static void StopListen()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
            }
        }
    }
}
