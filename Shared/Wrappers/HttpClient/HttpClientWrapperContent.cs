using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Shared.Wrappers.HttpClient;

public class HttpClientWrapperContent(HttpContent content) : IHttpClientResponseContent
{
    public Task<T?> ReadFromJsonAsync<T>() => content.ReadFromJsonAsync<T>();
}