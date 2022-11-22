using Shared;
using Shared.Logging.Interfaces;

namespace ConfigProviders;

public class LocalFileConfigProvider : BaseConfigProvider
{
    private static readonly string ConfigPath = AppContext.BaseDirectory + "config.ini";

    public LocalFileConfigProvider(ILogger logger): base(logger)
    {
    }

    protected override Uri GetConfigInternal()
    {
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
            return new UriBuilder(Scheme, Host, Port).Uri;
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
                        Host = value;
                        break;
                    case PortName:
                        if(int.TryParse(value, out var port))
                            Port = port;
                        break;
                    case SchemeName:
                        Scheme = value;
                        break;
                }
            }
        }

        try
        {
            return new UriBuilder(Scheme, Host, Port).Uri;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    protected override void SetConfigInternal(Uri appConfig)
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
            throw;
        }
    }
}
