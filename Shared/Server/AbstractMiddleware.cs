using Shared.DataObjects.Http;

namespace Shared.Server;

public abstract class AbstractMiddleware
{
    protected readonly HttpEventHandler? Next;

    protected AbstractMiddleware(HttpEventHandler? next = null)
    {
        Next = next;
    }

    public abstract void ProcessRequest(Context context);
}