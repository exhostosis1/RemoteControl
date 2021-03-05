using RemoteControl.Core.Abstract;
using RemoteControl.Core.Controllers;
using RemoteControl.Core.Interfaces;
using RemoteControl.Core.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace RemoteControl.Core
{
    public class Main
    {
        private readonly IListener _httplistener;

        public bool IpLookup;
        
        public event EventHandler<IEnumerable<string>> IpChanged;

        private const string DefaultScheme = "http";
        private const int DefaultPort = 1488;

        private readonly string _defaultUri = $"{DefaultScheme}://localhost:{DefaultPort}/";

        public Main(bool ipLookup)
        {
            IpLookup = ipLookup;

            AbstractController apiController = new ApiController();
            AbstractController httpController = new HttpController();

            _httplistener = new MyHttpListener();
            _httplistener.OnHttpRequest += httpController.ProcessRequest;
            _httplistener.OnApiRequest += apiController.ProcessRequest;

            NetworkChange.NetworkAddressChanged += IpsChanged;
        }
    
        private void IpsChanged(object sender, EventArgs args)
        {
            if (IpLookup)
            {
                Stop();

                var uris = GetCurrentUris();
                if (uris.Length > 0)
                    uris = new[] { _defaultUri };

                Start(uris);
            }
        }

        public IEnumerable<string> GetCurrentIps() => Dns.GetHostAddresses(Dns.GetHostName())
                .Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(x => x.ToString());

        public string[] GetCurrentUris() => GetCurrentIps()
            .Select(x => new UriBuilder(DefaultScheme, x, DefaultPort).ToString()).ToArray();

        public void Start(IReadOnlyCollection<string> uris)
        {
            if (uris == null || uris.Count == 0)
                uris = GetCurrentUris();

            _httplistener.StartListen(uris);

            IpChanged?.Invoke(this, uris);
        }

        public void Stop()
        {
            _httplistener.StopListen();
        }
    }
}