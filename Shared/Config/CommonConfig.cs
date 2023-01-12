using System.Text.Json.Serialization;

namespace Shared.Config;

[JsonDerivedType(typeof(ServerConfig), "server")]
[JsonDerivedType(typeof(BotConfig), "bot")]
public abstract class CommonConfig
{
    public string Name { get; set; } = string.Empty;
    public bool Autostart { get; set; }
}