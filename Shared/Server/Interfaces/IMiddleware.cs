using Shared.DataObjects.Interfaces;

namespace Shared.Server.Interfaces;

public interface IMiddleware
{
    public void ProcessRequest(IContext context);
}