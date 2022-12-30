using System.Collections.Generic;

namespace Shared.Config;

public class AppConfig
{
    public Dictionary<string, ServerConfig> Servers { get; set; } = new();
    public Dictionary<string, BotConfig> Bots { get; set; } = new();
}