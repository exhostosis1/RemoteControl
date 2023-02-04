using Shared.DataObjects;
using Shared.DataObjects.Bot;
using Shared.DataObjects.Web;

namespace Shared.Server;

public interface IMiddlewareChain<T> where T: IContext
{
    public void ChainRequest(T context);
}

public interface IBotMiddlewareChain : IMiddlewareChain<BotContext>
{
}

public interface IWebMiddlewareChain : IMiddlewareChain<WebContext>
{
}