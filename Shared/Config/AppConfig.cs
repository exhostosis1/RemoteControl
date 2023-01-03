using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Shared.Config;

public class AppConfig
{
    public List<ServerConfig> Servers { get; set; } = new();
    public List<BotConfig> Bots { get; set; } = new();

    [JsonIgnore]
    public IEnumerable<CommonConfig> All => Servers.Concat<CommonConfig>(Bots);
}