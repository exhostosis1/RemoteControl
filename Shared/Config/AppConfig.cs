using System.Collections.Generic;
using System.Linq;

namespace Shared.Config;

public class AppConfig
{
    public List<CommonConfig> Items { get; set; } = new();
    public IEnumerable<ServerConfig> Servers => Items.Where(x => x is ServerConfig).Cast<ServerConfig>();
    public IEnumerable<BotConfig> Bots => Items.Where(x => x is BotConfig).Cast<BotConfig>();

    public AppConfig(){}

    public AppConfig(SerializableAppConfig config)
    {
        Items = config.Servers.Concat<CommonConfig>(config.Bots).ToList();
    }

    public AppConfig(IEnumerable<CommonConfig> items) => Items = items.ToList();
}