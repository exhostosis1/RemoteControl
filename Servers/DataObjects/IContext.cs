namespace Servers.DataObjects;

public interface IContext
{
    public IRequest Request { get; }
    public IResponse Response { get; }
}