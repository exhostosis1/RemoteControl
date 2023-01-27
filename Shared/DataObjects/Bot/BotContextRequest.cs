using System;

namespace Shared.DataObjects.Bot;

public class BotContextRequest : IRequest
{
    public int Id { get; set; }
    public string Command { get; set; }

    public string ApiUrl { get; set; }
    public string ApiKey { get; set; }

    public DateTime Date { get; set; }

    public BotContextRequest(string apiUrl, string apiKey, int id, string command, DateTime date)
    {
        Id = id;
        Command = command;
        ApiUrl = apiUrl;
        ApiKey = apiKey;
        Date = date;
    }
}