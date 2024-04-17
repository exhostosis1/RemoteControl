using MainApp.Servers.DataObjects;

namespace MainApp.Servers.Middleware;

public interface IMiddleware
{
    public Task ProcessRequestAsync(RequestContext context, RequestDelegate next);
}

public delegate Task RequestDelegate(RequestContext context);