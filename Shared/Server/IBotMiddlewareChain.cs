using Shared.DataObjects.Bot;

namespace Shared.Server;

public interface IBotMiddlewareChain: IMiddlewareChain<BotContext>
{
}