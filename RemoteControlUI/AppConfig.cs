using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace RemoteControlUI
{
    internal class AppConfig
    {
        const string FileName = "config.ini";

        const string PortConfigName = "port";
        const string IPConfigName = "host";
        const string SchemeConfigName = "scheme";
        const string SimpleConfigName = "simple";

        const char EqualityChar = '=';

        public static string DefaultScheme => "http";
        public static string DefaultHost => Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();
        public static int DefaultPort => 80;
        public static bool DefaultSimple => false;

        private static readonly Dictionary<string, string> config = new Dictionary<string, string>();

        public int Port;
        public string Host;
        public string Scheme;
        public bool Simple;

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

        public AppConfig(string scheme, string host, int port, bool simple)
        {
            Port = port;
            Host = host;
            Simple = simple;
            Scheme = scheme;
        }

        internal static AppConfig GetServerConfig()
        {
            if (!Int32.TryParse(GetAppConfig(PortConfigName), out var port))
            {
                port = DefaultPort;
            }

            var scheme = GetAppConfig(SchemeConfigName) ?? DefaultScheme;
            var host = GetAppConfig(IPConfigName) ?? DefaultHost;
            
            if(!Boolean.TryParse(GetAppConfig(SimpleConfigName), out var simple))
            {
                simple = DefaultSimple;
            }

            return new AppConfig(scheme, host, port, simple);
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
