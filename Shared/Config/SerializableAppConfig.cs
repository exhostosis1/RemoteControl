using System.Collections.Generic;
using System.Linq;

namespace Shared.Config;

public class SerializableAppConfig
{
    public List<ServerConfig> Servers { get; set; } = new();
    public List<BotConfig> Bots { get; set; } = new();

    public SerializableAppConfig(){}

    public SerializableAppConfig(AppConfig config)
    {
        Servers = config.Items.Where(x => x is ServerConfig).Cast<ServerConfig>().ToList();
        Bots = config.Items.Where(x => x is BotConfig).Cast<BotConfig>().ToList();
    }

    public SerializableAppConfig(IEnumerable<ServerConfig> servers, IEnumerable<BotConfig> bots)
    {
        Servers = servers.ToList();
        Bots = bots.ToList();
    }
}