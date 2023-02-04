using Shared.DataObjects.Web;

namespace Shared.Server;

public interface IWebMiddlewareChain: IMiddlewareChain<WebContext>
{
}