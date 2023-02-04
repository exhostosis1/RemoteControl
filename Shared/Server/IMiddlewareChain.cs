using Shared.DataObjects;

namespace Shared.Server;

public interface IMiddlewareChain<T> where T: IContext
{
    public void ChainRequest(T context);
}