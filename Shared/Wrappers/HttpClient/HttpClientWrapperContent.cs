using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Shared.Wrappers.HttpClient;

public class HttpClientWrapperContent : IHttpClientResponseContent
{
    private readonly HttpContent _content;

    public HttpClientWrapperContent(HttpContent content)
    {
        _content = content;
    }

    public Task<T?> ReadFromJsonAsync<T>() => _content.ReadFromJsonAsync<T>();
}