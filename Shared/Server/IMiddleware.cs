namespace Shared.Server;

public interface IMiddleware: IEndpoint
{
    public HttpEventHandler? Next { get; set; }
}