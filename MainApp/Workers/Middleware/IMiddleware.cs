using MainApp.Workers.DataObjects;

namespace MainApp.Workers.Middleware;

internal interface IMiddleware
{
    public Task ProcessRequestAsync(RequestContext context, RequestDelegate next);
}

internal delegate Task RequestDelegate(RequestContext context);