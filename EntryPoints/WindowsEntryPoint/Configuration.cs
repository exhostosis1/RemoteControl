using Shared.Config;

namespace WindowsEntryPoint;

internal static class Configuration
{
    private static readonly JsonConfigurationProvider Config; 

    static Configuration()
    {
        Config = new JsonConfigurationProvider(Path.Combine(Environment.CurrentDirectory, "appsettings.json"));
    }

    public static AppConfig GetServersConfigurations()
    {
        return Config.GetJson();
    }

    public static void SetServersConfigurations(AppConfig config)
    {
        Config.WriteJson(config);
    }
}