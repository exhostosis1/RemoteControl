using Shared.DataObjects;
using System.Threading.Tasks;

namespace Shared;

public interface IMiddleware
{
    public Task ProcessRequestAsync(IContext context, RequestDelegate next);
}

public delegate Task RequestDelegate(IContext context);