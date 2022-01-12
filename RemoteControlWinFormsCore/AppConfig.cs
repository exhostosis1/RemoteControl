namespace RemoteControl
{
    public static class AppConfig
    {
        private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";

        internal static IEnumerable<Uri> GetConfigUris()
        {
            if (!File.Exists(ConfigPath))
                return Enumerable.Empty<Uri>();

            try
            {
                return File.ReadAllLines(ConfigPath).Where(x => !x.StartsWith("//")).Distinct().Select(x => new Uri(x.Trim()));
            }
            catch(Exception e)
            {
                Logger.Log(e.Message);

                return Enumerable.Empty<Uri>();
            }
        }
    }
}
