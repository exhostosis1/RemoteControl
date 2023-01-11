using Shared.DataObjects.Http;

namespace Shared.Server;

public abstract class AbstractEndpoint
{
    public abstract void ProcessRequest(Context context);
}