namespace Shared.DataObjects;

public interface IResponse
{
    public void Close();
}

public interface IRequest
{

}

public interface IContext
{
    public IRequest Request { get; }
    public IResponse Response { get; }
}