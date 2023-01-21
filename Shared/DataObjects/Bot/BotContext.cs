namespace Shared.DataObjects.Bot;

public class BotContext: IContext
{
    public BotContextRequest Request { get; set; }
    public BotContextResponse Response { get; set; }

    public BotContext(BotContextRequest request)
    {
        Request = request;
    }

    public BotContext(BotContextRequest request, BotContextResponse response) : this(request)
    {
        Response = response;
    }
}