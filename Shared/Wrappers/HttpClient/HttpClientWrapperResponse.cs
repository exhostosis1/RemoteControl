using Shared.Listeners;
using System.Net;

namespace Shared.Wrappers.HttpClient;

public class HttpClientWrapperResponse : IHttpClientResponse
{
    public bool IsSuccessStatusCode { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public IHttpClientResponseContent Content { get; set; }
}