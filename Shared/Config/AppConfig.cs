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

    public override bool Equals(object? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is not AppConfig that) return false;

        if (ProcessorConfigs.Count != that.ProcessorConfigs.Count) return false;

        return !ProcessorConfigs.Where((t, i) => !ProcessorConfigs[i].Equals(that.ProcessorConfigs[i])).Any();
    }
}