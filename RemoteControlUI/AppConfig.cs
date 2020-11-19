using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoteControl
{
    internal static class AppConfig
    {
        private const string FileName = "config.ini";

        private const string PortConfigName = "port";
        private const string IpConfigName = "host";
        private const string SchemeConfigName = "scheme";
        private const string SimpleConfigName = "simple";
        private const string SocketConfigName = "socket";
        private const string ApiHostName = "apihost";
        private const string ApiPortName = "apiport";
        private const string ApiSchemeName = "apischeme";

        private const char EqualityChar = '=';

        private static string DefaultScheme => "http";
        private static int DefaultPort => 80;
        private static int DefaultApiPort => Port + 1;
        private static bool DefaultSimple => false;
        private static bool DefaultSocket => false;
        private static string DefaultApiScheme => "ws";

        private static readonly Dictionary<string, string> Config = new Dictionary<string, string>();

        public static int Port
        {
            get
            {
                if (!int.TryParse(GetAppConfig(PortConfigName), out var port)) port = DefaultPort;
                return port;
            }
            set => SetAppConfig(PortConfigName, value.ToString());
        }

        public static string Host
        {
            get => GetAppConfig(IpConfigName);
            set => SetAppConfig(IpConfigName, value);
        }

        public static string Scheme
        {
            get => GetAppConfig(SchemeConfigName) ?? DefaultScheme;
            set => SetAppConfig(SchemeConfigName, value);
        }

        public static bool Simple
        {
            get
            {
                if (!bool.TryParse(GetAppConfig(SimpleConfigName), out var simple)) simple = DefaultSimple;
                return simple;
            }
            set => SetAppConfig(SimpleConfigName, value.ToString());
        }

        public static bool Socket
        {
            get
            {
                if (!bool.TryParse(GetAppConfig(SocketConfigName), out var socket)) socket = DefaultSocket;
                return socket;
            }
            set => SetAppConfig(SocketConfigName, value.ToString());
        }

        public static int ApiPort
        {
            get
            {
                if (!int.TryParse(GetAppConfig(ApiPortName), out var apiport)) apiport = DefaultApiPort;
                return apiport;
            }
            set => SetAppConfig(ApiPortName, value.ToString());
        }

        public static string ApiHost
        {
            get => GetAppConfig(ApiHostName);
            set => SetAppConfig(ApiHostName, value);
        }

        public static string ApiScheme
        {
            get => GetAppConfig(ApiSchemeName) ?? DefaultApiScheme;
            set => SetAppConfig(ApiSchemeName, value);
        }

        static AppConfig()
        {
            if (!File.Exists(FileName))
                return;

            var lines = File.ReadAllLines(FileName).Where(x => x.Trim() != string.Empty);

            foreach (var line in lines)
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
            File.WriteAllLines(FileName, Config.Select(x => $"{x.Key} {EqualityChar} {x.Value}"));
        }
    }
}
