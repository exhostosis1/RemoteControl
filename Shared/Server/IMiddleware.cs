using System;
using System.Threading.Tasks;
using Shared.DataObjects;

namespace Shared.Server;

public interface IMiddleware
{
    public Task ProcessRequestAsync(IContext context, Func<IContext, Task> next);
}