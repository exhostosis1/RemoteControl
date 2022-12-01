using System.Collections.Generic;

namespace Shared.Config;

public class BotConfig
{
    public List<int> ChatIds { get; set; } = new();
    public bool StartListening { get; set; } = true;
}