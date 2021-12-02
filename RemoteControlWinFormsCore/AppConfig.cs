using RemoteControl;

namespace RemoteControlWinFormsCore
{
    public static class AppConfig
    {
        private static readonly ILogger _logger = Logger.GetFileLogger(typeof(AppConfig));

        private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";

        public const string DefaultScheme = "http";
        public const string DefaultHost = "localhost";
        public const int DefaultPort = 1488;

        public static Uri? CurrentUri { get; set; }
        public static Uri? PrefUri { get; set; }

        static AppConfig()
        {
            ReadFile();
        }

        private static void ReadFile()
        {
            if (!File.Exists(ConfigPath))
                return;

            var lines = File.ReadAllLines(ConfigPath);

            var host = string.Empty;
            var port = 0;

            foreach (var line in lines)
            {
                string param, value;

                try
                {
                    var split = line.Split(':');
                    param = split[0].Trim();
                    value = split[1].Trim();
                }
                catch(Exception e)
                {
                    _logger.Log(ErrorLevel.Error, e.Message);

                    continue;
                }

                switch (param)
                {
                    case "host":
                        host = value;
                        break;
                    case "port":
                        _ = int.TryParse(value, out port);
                        break;
                    default:
                        break;
                }
            }

            PrefUri = new UriBuilder(DefaultScheme, string.IsNullOrEmpty(host) ? DefaultHost : host,
                port == 0 ? DefaultPort : port).Uri;
        }
    }
}
