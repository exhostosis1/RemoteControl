namespace Shared.DataObjects.Bot;

public class BotContextRequest
{
    public int Id { get; set; }
    public string Command { get; set; }

    public string ApiUrl { get; set; }
    public string ApiKey { get; set; }

    public BotContextRequest(string apiUrl, string apiKey, int id, string command)
    {
        Id = id;
        Command = command;
        ApiUrl = apiUrl;
        ApiKey = apiKey;
    }
}