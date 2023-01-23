namespace Shared.DataObjects.Http;

public class HttpContextRequest : IRequest
{
    public string Path { get; set; }

    public HttpContextRequest(string path) => Path = path;
}