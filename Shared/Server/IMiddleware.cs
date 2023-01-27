using Shared.DataObjects;
using Shared.DataObjects.Bot;
using Shared.DataObjects.Web;

namespace Shared.Server;

public interface IMiddleware<in T> where T : IContext
{
    public void ProcessRequest(T context);
}

public interface IWebMiddleware : IMiddleware<WebContext>
{
}

public interface IBotMiddleware : IMiddleware<BotContext>
{
}