using System.Net;

namespace RemoteControl.App.Web.Listeners
{
    internal static class MyHttpListener
    {
        private static HttpListener _listener = new();
        public static bool IsListening => _listener.IsListening;
        public static IEnumerable<string> ListeningUris => _listener.Prefixes;

        public static event HttpEventHandler? OnRequest;

        public static void StartListen(params Uri[] urls)
        {
            if (urls.Length == 0) return;

            if(_listener.IsListening)
                _listener.Stop();

            _listener.Prefixes.Clear();

            foreach (var url in urls)
            {
                _listener.Prefixes.Add(url.ToString());
            }

            _listener.Start();

            Task.Run(ProcessRequest);
        }

        private static async Task ProcessRequest()
        {
            while (true)
            {
                try
                {
                    var context = await _listener.GetContextAsync();

                    OnRequest?.Invoke(context);
                    context.Response.Close();
                }
                catch (ObjectDisposedException)
                {
                    _listener = new HttpListener();
                    return;
                }
                catch
                {
                    if (!_listener.IsListening)
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
