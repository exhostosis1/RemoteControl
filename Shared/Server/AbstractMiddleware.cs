using Shared.DataObjects.Http;

namespace Shared.Server;

public abstract class AbstractMiddleware: IMiddleware
{
    public HttpEventHandler? Next { get; set; }

    protected AbstractMiddleware(HttpEventHandler? next = null)
    {
        Next = next;
    }

    public abstract void ProcessRequest(Context context);
} 