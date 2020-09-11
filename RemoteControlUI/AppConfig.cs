using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace RemoteControl
{
    internal static class AppConfig
    {
        private const string fileName = "config.ini";

        private const string portConfigName = "port";
        private const string ipConfigName = "host";
        private const string schemeConfigName = "scheme";
        private const string simpleConfigName = "simple";
        private const string socketConfigName = "socket";
        private const string apiHostName = "apihost";
        private const string apiPortName = "apiport";
        private const string apiSchemeName = "apischeme";

        private const char equalityChar = '=';

        private static string DefaultScheme => "http";

        private static string DefaultHost => Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();

        private static int DefaultPort => 80;
        private static int DefaultApiPort => Port + 1;
        private static string DefaultApiHost => Host;
        private static bool DefaultSimple => false;
        private static bool DefaultSocket => false;
        private static string DefaultApiScheme => "ws";

        private static readonly Dictionary<string, string> Config = new Dictionary<string, string>();

        public static int Port
        {
            get
            {
                if (!int.TryParse(GetAppConfig(portConfigName), out var port)) port = DefaultPort;
                return port;
            }
            set => SetAppConfig(portConfigName, value.ToString());
        }

        public static string Host
        {
            get => GetAppConfig(ipConfigName) ?? DefaultHost;
            set => SetAppConfig(ipConfigName, value);
        }

        public static string Scheme
        {
            get => GetAppConfig(schemeConfigName) ?? DefaultScheme;
            set => SetAppConfig(schemeConfigName, value);
        }

        public static bool Simple
        {
            get
            {
                if (!bool.TryParse(GetAppConfig(simpleConfigName), out var simple)) simple = DefaultSimple;
                return simple;
            }
            set => SetAppConfig(simpleConfigName, value.ToString());
        }

        public static bool Socket
        {
            get
            {
                if (!bool.TryParse(GetAppConfig(socketConfigName), out var socket)) socket = DefaultSocket;
                return socket;
            }
            set => SetAppConfig(socketConfigName, value.ToString());
        }

        public static int ApiPort
        {
            get
            {
                if (!int.TryParse(GetAppConfig(apiPortName), out var apiport)) apiport = DefaultApiPort;
                return apiport;
            }
            set => SetAppConfig(apiPortName, value.ToString());
        }

        public static string ApiHost
        {
            get => GetAppConfig(apiHostName) ?? DefaultApiHost;
            set => SetAppConfig(apiHostName, value);
        }

        public static string ApiScheme
        {
            get => GetAppConfig(apiSchemeName) ?? DefaultApiScheme;
            set => SetAppConfig(apiSchemeName, value);
        }

        static AppConfig()
        {
            if (!File.Exists(fileName))
                return;

            var lines = File.ReadAllLines(fileName).Where(x => x.Trim() != string.Empty);

            foreach (var line in lines)
            {
                var index = line.IndexOf(equalityChar);
                if (index == -1)
                    continue;

                var name = line.Substring(0, index).Trim();
                var value = line.Substring(index + 1).Trim();

                if (name != string.Empty && value != string.Empty)
                    Config.Add(name.ToLower(), value);
            }
        }

        private static string GetAppConfig(string name)
        {
            name = name.ToLower();

            return Config.ContainsKey(name) ? Config[name] : null;
        }

        private static void SetAppConfig(string name, string value)
        {
            name = name.ToLower();

            if (Config.ContainsKey(name))
            {
                Config[name] = value;
            }
            else
            {
                Config.Add(name, value);
            }

        }

        internal static void WriteConfigToFile()
        {
            File.WriteAllLines(fileName, Config.Select(x => $"{x.Key} {equalityChar} {x.Value}"));
        }
    }
}
