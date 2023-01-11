using System.Collections.Generic;

namespace Shared.DataObjects.Bot;

public class BotContext
{
    public int Id { get; set; }
    public string Message { get; set; }
    public string? Result { get; set; }
    public IEnumerable<IEnumerable<string>>? Buttons { get; set; }

    public BotContext(int id, string message)
    {
        Id = id;
        Message = message;
    }
}