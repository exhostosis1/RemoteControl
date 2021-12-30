namespace RemoteControl
{
    public static class AppConfig
    {
        private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";

        public const string DefaultScheme = "http";
        public const int DefaultPort = 1488;

        public static Uri[] PrefUris { get; private set; } = Array.Empty<Uri>();

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
            catch
            {
                throw new Exception("Cannot read config");
            }
        }
    }
}
