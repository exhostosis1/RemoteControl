using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Wrappers.HttpClient;

public class HttpClientWrapper : IHttpClient
{
    private readonly System.Net.Http.HttpClient _client = new()
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
        newRequest.Content = new StringContent(request.Content ?? string.Empty, Encoding.UTF8, "application/json");

        var response = _client.Send(newRequest);

        return new HttpClientWrapperResponse
        {
            IsSuccessStatusCode = response.IsSuccessStatusCode,
            StatusCode = response.StatusCode,
            Content = new HttpClientWrapperContent(response.Content)
        };
    }
}