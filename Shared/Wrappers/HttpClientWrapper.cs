using Shared.Listeners;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Wrappers;

public class HttpClientWrapperRequest: IHttpClientRequest
{
    public HttpMethod Method { get; set; }
    public string RequestUri { get; set; }
    public string Content { get; set; }
    public IHttpClientRequest New(HttpMethod method, string requestUri)
    {
        throw new NotImplementedException();
    }
}

public class HttpClientWrapperResponse: IHttpClientResponse
{
    public bool IsSuccessStatusCode { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public IHttpClientResponseContent Content { get; set; }
}

public class HttpClientWrapperContent: IHttpClientResponseContent
{
    private readonly HttpContent _content;

    public HttpClientWrapperContent(HttpContent content)
    {
        _content = content;
    }

    public Task<T?> ReadFromJsonAsync<T>() => _content.ReadFromJsonAsync<T>();
}

public class HttpClientWrapper : IHttpClient
{
    private readonly HttpClient _client = new()
    {
        Timeout = TimeSpan.FromSeconds(15)
    };

    public Task<T> PostAsJsonAndGetResultAsync<T>(string? uri, object parameters, JsonSerializerOptions? options = null,
        CancellationToken token = default) => _client.PostAsJsonAndGetResultAsync<T>(uri, parameters, options, token);

    public Task PostAsJsonAsync(string? uri, object parameters, JsonSerializerOptions? options = null,
        CancellationToken token = default) => _client.PostAsJsonAsync(uri, parameters, options, token);

    public IHttpClientResponse Send(IHttpClientRequest request)
    {
        var newRequest = new HttpRequestMessage(request.Method, request.RequestUri);
        newRequest.Content = new StringContent(request.Content, Encoding.UTF8, "application/json");

        var response = _client.Send(newRequest);

        return new HttpClientWrapperResponse
        {
            IsSuccessStatusCode = response.IsSuccessStatusCode,
            StatusCode = response.StatusCode,
            Content = new HttpClientWrapperContent(response.Content)
        };
    }
}