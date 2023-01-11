using System.Net;

namespace Shared.DataObjects.Http;

public class Request
{
    public string Path { get; set; }

    public Request(string path) => Path = path;

    public Request(HttpListenerRequest request)
    {
        Path = request.RawUrl ?? string.Empty;
    }
}