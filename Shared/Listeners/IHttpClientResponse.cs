using System.Net;

namespace Shared.Listeners;

public interface IHttpClientResponse
{
    public bool IsSuccessStatusCode { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public IHttpClientResponseContent? Content { get; set; }
}