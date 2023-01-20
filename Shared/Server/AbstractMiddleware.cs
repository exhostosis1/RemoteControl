using Shared.DataObjects;

namespace Shared.Server;

public abstract class AbstractMiddleware<T> where T: IContext
{
    protected AbstractMiddleware<T>? Next { get; set; }

    public AbstractMiddleware(AbstractMiddleware<T>? next = null)
    {
        Next = next;
    }

    public abstract void ProcessRequest(T context);
}