namespace Servers.DataObjects.Bot;

public class BotContext(BotContextRequest request, BotContextResponse response) : IContext
{
    public IRequest Request => BotRequest;
    public IResponse Response => BotResponse;

    public BotContextRequest BotRequest { get; set; } = request;
    public BotContextResponse BotResponse { get; set; } = response;
}