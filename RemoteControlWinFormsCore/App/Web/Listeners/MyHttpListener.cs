﻿using System.Net;

namespace RemoteControl.App.Web.Listeners
{
    internal static class MyHttpListener
    {
        private static HttpListener? _listener;
        public static bool IsListening => _listener?.IsListening ?? false;
        public static IEnumerable<string> ListeningUris => _listener?.Prefixes ?? Enumerable.Empty<string>();

        public static event HttpEventHandler? OnRequest;

        public static void StartListen(params Uri[] urls)
        {
            if (urls.Length == 0) return;

            StopListen();
            _listener = new HttpListener();

            foreach(var url in urls)
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
                    if (_listener == null) return;

                    var context = await _listener.GetContextAsync();

                    OnRequest?.Invoke(context);
                    context?.Response.Close();
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch
                {
                    if (!(_listener?.IsListening ?? false))
                        return;
                }
            }
        }

        public static void StopListen()
        {
            if (_listener?.IsListening ?? false)
            {
                _listener.Stop();
            }

            _listener = null;
        }
    }
}
