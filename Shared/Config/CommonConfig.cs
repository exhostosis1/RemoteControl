using System.Text.Json.Serialization;

namespace Shared.Config;

[JsonDerivedType(typeof(WebConfig), "web")]
[JsonDerivedType(typeof(BotConfig), "bot")]
public abstract class CommonConfig
{
    public string Name { get; set; } = string.Empty;
    public bool AutoStart { get; set; }
}