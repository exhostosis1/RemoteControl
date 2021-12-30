using System.Net;

namespace RemoteControl.App.Web.Listeners
{
    internal static class MyHttpListener
    {
        private static readonly HttpListener Listener = new();
        public static bool IsListening => Listener.IsListening;
        public static IEnumerable<string> ListeningUris => Listener.Prefixes;

        public static event HttpEventHandler? OnRequest;

        public static async Task StartListenAsync(params Uri[] urls)
        {
            if (urls.Length == 0) return;

            StopListen();

            Listener.Prefixes.Clear();

            foreach(var url in urls)
            {
                Listener.Prefixes.Add(url.ToString());
            }

            Listener.Start();

            while (true)
            {
                try
                {
                    var context = await Listener.GetContextAsync();

                    OnRequest?.Invoke(context);
                    context?.Response.Close();
                }
                catch
                {
                    if (!Listener.IsListening)
                        return;
                }
            }
        }                

        public static void StopListen()
        {
            if (Listener.IsListening)
            {
                Listener.Stop();
            }
        }
    }
}
