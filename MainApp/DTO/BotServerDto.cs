using MainApp.Interfaces;

namespace MainApp.DTO;

public class BotServerDto: IBotServer
{
    public string Name { get; set; } = "";
    public Guid Id { get; set; }
    public bool Status { get; set; }
    public bool IsAutostart { get; set; }
    public Uri? ApiUri { get; set; }
    public string ApiKey { get; set; } = "";
    public IReadOnlyList<string> Usernames { get; set; } = [];
}