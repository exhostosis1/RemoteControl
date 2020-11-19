using RemoteControl.Core.Abstract;
using RemoteControl.Core.Controllers;
using RemoteControl.Core.Interfaces;
using RemoteControl.Core.Listeners;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace RemoteControl.Core
{
    public class Main
    {
        private readonly IListener _httplistener;
        public string Schema { get; set; }
        public int Port { get; set; }

        public bool IpLookup;

        private string[] _uris;
        
        public event EventHandler<string[]> IpChanged;

        public Main(string schema, int port, bool ipLookup)
        {
            Schema = schema;
            Port = port;
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
                var hosts = GetCurrentIps();

                Stop();
                Start(hosts);
            }
        }

        private string[] GetCurrentIps() => Dns.GetHostAddresses(Dns.GetHostName())
                .Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(x => x.ToString()).ToArray();

        public void Start(params string[] hosts)
        {
            Start(Schema, Port, hosts);
        }

        public void Start(string schema, int port, params string[] hosts)
        {
            Schema = schema;
            Port = port;

            if (IpLookup)
            {
                hosts = GetCurrentIps();
            }

            _uris = hosts.Select(x => new UriBuilder(Schema, x, Port).ToString()).ToArray();

            _httplistener.StartListen(_uris);

            IpChanged?.Invoke(this, _uris);
        }

        public void Stop()
        {
            _httplistener.StopListen();
        }
    }
}