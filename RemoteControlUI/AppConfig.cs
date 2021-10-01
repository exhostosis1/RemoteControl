using System;
using System.IO;

namespace RemoteControl
{
    public static class AppConfig
    {
        private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";

        public static string Scheme = "http";
        public static string Host = "localhost";
        public static int Port = 1488;

        public static Uri Uri => new UriBuilder(Scheme, Host, Port).Uri;

        static AppConfig()
        {
            ReadFile();
        }

        public static void SetFromString(string input)
        {
            var uri = new Uri(input);

            Host = uri.Host;
            Scheme = uri.Scheme;
            Port = uri.Port;
        }

        private static void ReadFile()
        {
            if (!File.Exists(ConfigPath))
                return;

            var lines = File.ReadAllLines(ConfigPath);

            foreach (var line in lines)
            {
                string param, value;

                try
                {
                    var split = line.Split(':');
                    param = split[0].Trim();
                    value = split[1].Trim();
                }
                catch
                {
                    continue;
                }

                switch (param)
                {
                    case "host":
                        Host = value;
                        break;
                    case "port":
                        int.TryParse(value, out Port);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
