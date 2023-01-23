namespace Shared.DataObjects.Bot;

public class BotContext : IContext
{
    public IRequest Request => BotRequest;
    public IResponse Response => BotResponse;

    public BotContextRequest BotRequest { get; set; }
    public BotContextResponse BotResponse { get; set; }

    public BotContext(BotContextRequest request, BotContextResponse response)
    {
        BotRequest = request;
        BotResponse = response;
    }
}