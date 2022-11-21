using Shared;
using Shared.Config;
using Shared.Logging.Interfaces;

namespace ConfigProviders;

public class LocalFileConfigProvider : BaseConfigProvider
{
    private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";

    public LocalFileConfigProvider(ILogger logger): base(logger)
    {
    }

    protected override AppConfig GetConfigInternal()
    {
        var result = new AppConfig();

        IEnumerable<string> lines;

        try
        {
            lines = File.ReadAllLines(ConfigPath)
                .Select(x => x.Contains('#') ? x[..x.IndexOf('#')].Trim() : x.Trim())
                .Where(x => !string.IsNullOrEmpty(x));
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            return result;
        }

        foreach (var line in lines)
        {
            if (line.Contains('='))
            {
                if (!line.TryParseConfig(out var param, out var value))
                    continue;

                switch (param)
                {
                    case HostName:
                        result.Host = value;
                        break;
                    case PortName:
                        if(int.TryParse(value, out var port))
                            result.Port = port;
                        break;
                    case SchemeName:
                        result.Scheme = value;
                        break;
                }
            }
        }

        return result;
    }

    protected override void SetConfigInternal(AppConfig appConfig)
    {
        var result = new []
        {
            $"{SchemeName} = {appConfig.Scheme}",
            $"{HostName} = {appConfig.Host}",
            $"{PortName} = {appConfig.Port}"
        };

        try
        {

            File.WriteAllLines(ConfigPath, result);
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }
    }
}
