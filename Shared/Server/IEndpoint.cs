using Shared.DataObjects.Http;

namespace Shared.Server;

public interface IEndpoint
{
    public void ProcessRequest(Context context);
}