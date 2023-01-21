using Shared.DataObjects;

namespace Shared.Server;

public interface IMiddleware<in T> where T: IContext
{
    public void ProcessRequest(T context);
}

public abstract class AbstractMiddleware<T>: IMiddleware<T> where T : IContext
{
    protected IMiddleware<T>? Next { get; set; }

    public AbstractMiddleware(IMiddleware<T>? next = null)
    {
        Next = next;
    }

    public abstract void ProcessRequest(T context);
}