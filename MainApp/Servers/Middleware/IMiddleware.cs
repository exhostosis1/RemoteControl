using MainApp.Servers.DataObjects;

namespace MainApp.Servers.Middleware;

internal interface IMiddleware
{
    public Task ProcessRequestAsync(RequestContext context, RequestDelegate next);
}

internal delegate Task RequestDelegate(RequestContext context);