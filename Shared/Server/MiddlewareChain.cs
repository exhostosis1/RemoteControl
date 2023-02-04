using Shared.DataObjects;
using Shared.DataObjects.Bot;
using Shared.DataObjects.Web;

namespace Shared.Server;

public class MiddlewareChain<T>: IMiddlewareChain<T> where T: IContext
{
    private readonly IMiddleware<T>[] _middleware;

    public MiddlewareChain(IMiddleware<T>[] middleware)
    {
        _middleware = middleware;
        
        for (var i = 0; i < _middleware.Length - 1; i++)
        {
            _middleware[i].OnNext += _middleware[i + 1].ProcessRequest;
        }
    }

    public void ChainRequest(T context)
    {
        _middleware[0].ProcessRequest(null, context);
    }
}

public class WebMiddlewareChain: MiddlewareChain<WebContext>, IWebMiddlewareChain
{
    // ReSharper disable once CoVariantArrayConversion
    public WebMiddlewareChain(IWebMiddleware[] middleware) : base(middleware)
    {
    }
}

public class BotMiddlewareChain: MiddlewareChain<BotContext>, IBotMiddlewareChain
{
    // ReSharper disable once CoVariantArrayConversion
    public BotMiddlewareChain(IBotMiddleware[] middleware) : base(middleware)
    {
    }
}