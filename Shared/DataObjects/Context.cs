using Shared.DataObjects.Interfaces;

namespace Shared.DataObjects;

public class Context: IContext
{
    public IRequest Request { get; set; }
    public IResponse Response { get; set; } = new Response();

    public Context(string path) => Request = new Request(path);
}