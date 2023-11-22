using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Shared.Wrappers.HttpClient;

public class HttpClientWrapperContent(HttpContent content) : IHttpClientResponseContent
{
    private readonly HttpContent _content = content;

    public Task<T?> ReadFromJsonAsync<T>() => _content.ReadFromJsonAsync<T>();
}