using System.Collections.Generic;
using System.Linq;

namespace Shared.Config;

public class AppConfig
{
    public List<CommonConfig> Items { get; set; } = new();

    public AppConfig(){}

    public AppConfig(SerializableAppConfig config)
    {
        Items = config.Servers.Concat<CommonConfig>(config.Bots).ToList();
    }

    public AppConfig(IEnumerable<CommonConfig> items) => Items = items.ToList();
}