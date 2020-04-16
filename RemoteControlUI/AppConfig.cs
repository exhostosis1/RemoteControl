using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;


namespace RemoteControlUI
{
    internal static class AppConfig
    {
        const string FileName = "config.ini";

        const string PortConfigName = "port";
        const string IPConfigName = "host";
        const string SchemeConfigName = "scheme";

        const char EqualityChar = '=';

        public static string DefaultScheme => "http";
        public static string DefaultHost => Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();
        public static int DefaultPort => 80;

        private static readonly Dictionary<string, string> config = new Dictionary<string, string>();

        static AppConfig()
        {
            if (!File.Exists(FileName))
                return;

            var lines = File.ReadAllLines(FileName).Where(x => x.Trim() != string.Empty);
            var index = -1;

            var name = "";
            var value = "";

            foreach(var line in lines)
            {
                index = line.IndexOf(EqualityChar);
                if (index == -1)
                    continue;

                name = line.Substring(0, index).Trim();
                value = line.Substring(index + 1).Trim();

                if (name != string.Empty && value != string.Empty)
                    config.Add(name.ToLower(), value);
            }
        }

        internal static (string, string, string) GetServerConfig()
        {
            if (!Int32.TryParse(GetAppConfig(PortConfigName), out var port))
            {
                port = DefaultPort;
            }

            var scheme = GetAppConfig(SchemeConfigName) ?? DefaultScheme;
            var host = GetAppConfig(IPConfigName) ?? DefaultHost;

            return (scheme, host, port.ToString());
        }

        internal static string GetAppConfig(string name)
        {
            name = name.ToLower();

            if (config.ContainsKey(name))
                return config[name];
            else
                return null;
        }
    }
}
