namespace Servers.DataObjects.Bot;

public class BotContextRequest(string apiUrl, string apiKey, int id, string command, DateTime date) : IRequest
{
    public int Id { get; set; } = id;
    public string Command { get; set; } = command;

    public string ApiUrl { get; set; } = apiUrl;
    public string ApiKey { get; set; } = apiKey;

    public DateTime Date { get; set; } = date;
}