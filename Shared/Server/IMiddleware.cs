using Shared.DataObjects;

namespace Shared.Server;

public interface IMiddleware<in T> where T : IContext
{
    public void ProcessRequest(T context);
}