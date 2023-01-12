using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Shared.Config;

public class AppConfig
{
    public List<CommonConfig> ProcessorConfigs { get; set; } = new();
    
    [JsonIgnore]
    public IEnumerable<ServerConfig> Servers => ProcessorConfigs.Where(x => x is ServerConfig).Cast<ServerConfig>();
    
    [JsonIgnore]
    public IEnumerable<BotConfig> Bots => ProcessorConfigs.Where(x => x is BotConfig).Cast<BotConfig>();

    public AppConfig(){}

    public AppConfig(IEnumerable<CommonConfig> items) => ProcessorConfigs = items.ToList();
}