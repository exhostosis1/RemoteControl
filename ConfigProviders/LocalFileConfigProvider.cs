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

    protected override ConfigItem GetConfigInternal()
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

            return new ConfigItem
            {
                BotConfig = new BotConfig
                {
                    ChatIds = ChatIds,
                    StartListening = BotAutostart
                },
                ServerConfig = new ServerConfig
                {
                    Host = Host,
                    Port = Port,
                    Scheme = Scheme,
                    StartListening = ServerAutostart
                }
            };
        }

        foreach (var line in lines)
        {
            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                configSection = line[1..^1] switch
                {
                    ServerSectionName => ConfigSection.Server,
                    BotSectionName => ConfigSection.Bot,
                    _ => configSection
                };

                continue;
            }

            switch (configSection)
            {
                case ConfigSection.Server:
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
                                if (int.TryParse(value, out var port))
                                    Port = port;
                                break;
                            case SchemeName:
                                Scheme = value;
                                break;
                            case ServerAutostartName:
                                if (bool.TryParse(value, out var autostart))
                                    ServerAutostart = autostart;
                                break;
                        }
                    }

                    break;
                }
                case ConfigSection.Bot:
                {
                    if (line.Contains('='))
                    {
                        if (!line.TryParseConfig(out var param, out var value))
                            continue;

                        switch (param)
                        {
                            case BotAutostartName:
                                if (bool.TryParse(value, out var autostart))
                                    BotAutostart = autostart;
                                break;
                            case ChatIdsName:
                                ChatIds = value.Split(';',
                                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(x =>
                                {
                                    if(int.TryParse(x, out var res))
                                        return res;
                                    return -1;
                                }).Where(x => x != -1).ToList();
                                break;
                        }
                    }

                    break;
                }
                case ConfigSection.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        try
        {
            return new ConfigItem
            {
                BotConfig = new BotConfig
                {
                    ChatIds = ChatIds,
                    StartListening = BotAutostart
                },
                ServerConfig = new ServerConfig
                {
                    Host = Host,
                    Port = Port,
                    Scheme = Scheme,
                    StartListening = ServerAutostart
                }
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    protected override void SetConfigInternal(ConfigItem appConfig)
    {
        var result = new []
        {
            $"[{ServerSectionName}]",
            $"{SchemeName} = {appConfig.ServerConfig.Scheme}",
            $"{HostName} = {appConfig.ServerConfig.Host}",
            $"{PortName} = {appConfig.ServerConfig.Port}",
            $"{ServerAutostartName} = {appConfig.ServerConfig.StartListening}",
            $"",
            $"[{BotSectionName}",
            $"{ChatIdsName} = {string.Join(';', appConfig.BotConfig.ChatIds)}",
            $"{BotAutostartName} = {appConfig.BotConfig.StartListening}"
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
