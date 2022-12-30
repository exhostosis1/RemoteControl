using System.Collections.Generic;

namespace Shared.Config;

public class BotConfig: CommonConfig
{
    public string ApiUri { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public List<string> Usernames { get; set; } = new();
}