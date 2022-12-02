namespace Shared.Config;

public class AppConfig
{
    public ServerConfig ServerConfig { get; set; } = new();
    public BotConfig BotConfig { get; set; } = new();
}