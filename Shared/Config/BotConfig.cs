using System.ComponentModel;

namespace Shared.Config;

[DisplayName("Bot")]
public class BotConfig
{
    [DisplayName("ChatId")]
    public int ChatId { get; set; } = -1;

    [DisplayName("Autostart")]
    public bool StartListening { get; set; } = true;

    [DisplayName("ApiKey")]
    public string ApiKey { get; set; } = "";
}