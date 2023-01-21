namespace Shared.DataObjects.Http;

public class Request: IRequest
{
    public string Path { get; set; }

    public Request(string path) => Path = path;
}