namespace Shared.Server;

public interface IApiEndpoint: IEndpoint
{
    public string ApiVersion { get; }
}