using System.Net.Http;

namespace Shared.Listeners;

public interface IHttpClientRequest
{
    public HttpMethod Method { get; set; }
    public string RequestUri { get; set; }

    public string? Content { get; set; }
}