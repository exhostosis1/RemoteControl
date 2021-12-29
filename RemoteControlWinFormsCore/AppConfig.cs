using RemoteControl;

namespace RemoteControlWinFormsCore
{
    public static class AppConfig
    {
        private static readonly ILogger _logger = Logger.GetFileLogger(typeof(AppConfig));

        private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";

        public static readonly string DefaultScheme = "http";
        public static readonly int DefaultPort = 1488;

        public static Uri[] PrefUris { get; set; } = Array.Empty<Uri>();

        static AppConfig()
        {
            ReadFile();
        }

        private static void ReadFile()
        {
            if (!File.Exists(ConfigPath))
                return;

            try
            {
                PrefUris = File.ReadAllLines(ConfigPath).Where(x => !x.StartsWith("//")).Select(x => new Uri(x)).ToArray();
            }
            catch(Exception e)
            {
                _logger.Log(ErrorLevel.Error, e.Message);
                throw new Exception("Cannot read config");
            }
        }
    }
}
