using Shared.DataObjects;

namespace Shared.Server;

public abstract class AbstractMiddleware<T> : IMiddleware<T> where T : IContext
{
    protected IMiddleware<T>? Next { get; set; }

    public AbstractMiddleware(IMiddleware<T>? next = null)
    {
        Next = next;
    }

    public abstract void ProcessRequest(T context);
}