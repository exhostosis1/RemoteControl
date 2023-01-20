using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Shared.Config;

public class AppConfig: IEquatable<AppConfig>
{
    public List<CommonConfig> ProcessorConfigs { get; set; } = new();
    
    [JsonIgnore]
    public IEnumerable<ServerConfig> Servers => ProcessorConfigs.Where(x => x is ServerConfig).Cast<ServerConfig>();
    
    [JsonIgnore]
    public IEnumerable<BotConfig> Bots => ProcessorConfigs.Where(x => x is BotConfig).Cast<BotConfig>();

    public AppConfig(){}

    public AppConfig(IEnumerable<CommonConfig> items) => ProcessorConfigs = items.ToList();

    public bool Equals(AppConfig? that)
    {
        if (ReferenceEquals(null, that)) return false;
        if (ReferenceEquals(this, that)) return true;

        if (ProcessorConfigs.Count != that.ProcessorConfigs.Count) return false;

        return !ProcessorConfigs.Where((t, i) => !ProcessorConfigs[i].Equals(that.ProcessorConfigs[i])).Any();
    }
}