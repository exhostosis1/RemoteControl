using Shared.DataObjects.Http;
using System;

namespace Shared.Server;

public interface IMiddleware: IEndpoint
{
    public EventHandler<Context>? Next { get; set; }
}