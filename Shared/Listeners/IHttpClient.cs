using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Listeners;

public interface IHttpClient
{
    public Task<T> PostAsJsonAndGetResultAsync<T>(string? uri, object parameters, JsonSerializerOptions? options = null, CancellationToken token = default);
    public Task PostAsJsonAsync(string? uri, object parameters, JsonSerializerOptions? options = null, CancellationToken token = default);
    public IHttpClientResponse Send(IHttpClientRequest request);
}

public interface IHttpClientResponse
{
    public bool IsSuccessStatusCode { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public IHttpClientResponseContent Content { get; set; }
}

public interface IHttpClientResponseContent
{
    public Task<T?> ReadFromJsonAsync<T>();
}

public interface IHttpClientRequest
{
    public HttpMethod Method { get; set; }
    public string RequestUri { get; set; }

    public string Content { get; set; }
}
