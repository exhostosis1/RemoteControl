using Servers.DataObjects;

namespace Servers.Middleware;

public interface IMiddleware
{
    public Task ProcessRequestAsync(IContext context, RequestDelegate next);
}

public delegate Task RequestDelegate(IContext context);