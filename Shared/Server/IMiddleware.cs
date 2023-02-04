using System;
using Shared.DataObjects;
using Shared.DataObjects.Bot;
using Shared.DataObjects.Web;

namespace Shared.Server;

public interface IMiddleware<T> where T : IContext
{
    public event EventHandler<T> OnNext;
    public void ProcessRequest(object? _, T context);
}

public interface IWebMiddleware : IMiddleware<WebContext>
{
}

public interface IBotMiddleware : IMiddleware<BotContext>
{
}