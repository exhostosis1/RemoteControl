using System;
using Shared.DataObjects.Http;

namespace Shared.Server;

public abstract class AbstractMiddleware: IMiddleware
{
    public EventHandler<Context>? Next { get; set; }

    protected AbstractMiddleware(EventHandler<Context>? next = null)
    {
        Next = next;
    }

    public abstract void ProcessRequest(object? sender, Context context);
} 