using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace RemoteControl
{
    internal class AppConfig
    {
        private const string FileName = "config.ini";

        private const string PortConfigName = "port";
        private const string IpConfigName = "host";
        private const string SchemeConfigName = "scheme";
        private const string SimpleConfigName = "simple";

        private const char EqualityChar = '=';

        public static string DefaultScheme => "http";
        public static string DefaultHost => Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();
        public static int DefaultPort => 80;
        public static bool DefaultSimple => false;

        private static readonly Dictionary<string, string> Config = new Dictionary<string, string>();

        public int Port;
        public string Host;
        public string Scheme;
        public bool Simple;

        static AppConfig()
        {
            if (!File.Exists(FileName))
                return;

            var lines = File.ReadAllLines(FileName).Where(x => x.Trim() != string.Empty);

            foreach(var line in lines)
            {
                var index = line.IndexOf(EqualityChar);
                if (index == -1)
                    continue;

                var name = line.Substring(0, index).Trim();
                var value = line.Substring(index + 1).Trim();

                if (name != string.Empty && value != string.Empty)
                    Config.Add(name.ToLower(), value);
            }
        }

        public AppConfig(string scheme, string host, int port, bool simple)
        {
            Port = port;
            Host = host;
            Simple = simple;
            Scheme = scheme;
        }

        internal static AppConfig GetServerConfig()
        {            
            var scheme = GetAppConfig(SchemeConfigName) ?? DefaultScheme;
            var host = GetAppConfig(IpConfigName) ?? DefaultHost;

            if (!bool.TryParse(GetAppConfig(SimpleConfigName), out var simple)) simple = DefaultSimple;
            if (!int.TryParse(GetAppConfig(PortConfigName), out var port)) port = DefaultPort;         

            return new AppConfig(scheme, host, port, simple);
        }

        private static string GetAppConfig(string name)
        {
            name = name.ToLower();

            return Config.ContainsKey(name) ? Config[name] : null;
        }
    }
}
