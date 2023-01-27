using System.Net.Http;

namespace Shared.Wrappers.HttpClient;

public class HttpClientWrapperRequest : IHttpClientRequest
{
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public string RequestUri { get; set; } = string.Empty;
    public string? Content { get; set; } = string.Empty;
}