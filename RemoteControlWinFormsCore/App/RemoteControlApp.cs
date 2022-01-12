using RemoteControl.App.Web;
using RemoteControl.App.Web.Controllers;
using RemoteControl.App.Web.Listeners;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace RemoteControl.App
{
    public static class RemoteControlApp
    {
        private static bool _waitingForIp = true;
        public static string DefaultScheme = "http";
        public static int DefaultPort = 1488;

        public static event HttpEventHandler? OnRequest;

        static RemoteControlApp()
        {
            MyHttpListener.OnRequest += (context) =>
            {
                OnRequest?.Invoke(context);
                Router.ProcessRequest(context);
            };

            NetworkChange.NetworkAddressChanged += IpChanged;
        }

        public static IEnumerable<Uri> GetCurrentIPs() => Dns.GetHostAddresses(Dns.GetHostName())
            .Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(x => new UriBuilder(DefaultScheme, x.ToString(), DefaultPort).Uri);

        private static ICollection<Uri> GetUris(ICollection<Uri> prefUris)
        {
            if (prefUris.Count == 0)
            {
                return GetCurrentIPs().ToArray();
            }
            else
            {
                var ips = GetCurrentIPs().ToArray();

                var hosts = prefUris.Where(x => ips.Any(y => x.Host == y.Host)).ToArray();

                _waitingForIp = hosts.Length != prefUris.Count;

                return hosts;
            }
        }

        private static void IpChanged(object? sender, EventArgs args)
        {
            if (!_waitingForIp) return;

            Start(GetUris(Array.Empty<Uri>()));
        }

        public static void Start(ICollection<Uri> uris)
        {
            MyHttpListener.StartListen(GetUris(uris));
        }

        public static void Stop()
        {
            MyHttpListener.StopListen();
        }

        public static IEnumerable<Uri> GetListeningUris() => MyHttpListener.ListeningUris.Select(x => new Uri(x));

        public static bool IsListening => MyHttpListener.IsListening;
    }
}