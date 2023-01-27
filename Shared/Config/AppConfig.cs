using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Shared.Config;

public class AppConfig
{
    public List<CommonConfig> ServerConfigs { get; init; } = new();

    [JsonIgnore]
    public IEnumerable<WebConfig> Servers => ServerConfigs.Where(x => x is WebConfig).Cast<WebConfig>();

    [JsonIgnore]
    public IEnumerable<BotConfig> Bots => ServerConfigs.Where(x => x is BotConfig).Cast<BotConfig>();

    public AppConfig() { }

    public AppConfig(IEnumerable<CommonConfig> items) => ServerConfigs = items.ToList();

    public override bool Equals(object? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is not AppConfig that) return false;

        if (ServerConfigs.Count != that.ServerConfigs.Count) return false;

        return !ServerConfigs.Where((t, i) => !ServerConfigs[i].Equals(that.ServerConfigs[i])).Any();
    }

    protected bool Equals(AppConfig other)
    {
        return ServerConfigs.Equals(other.ServerConfigs);
    }

    public override int GetHashCode()
    {
        return ServerConfigs.GetHashCode();
    }
}