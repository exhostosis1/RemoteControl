using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Wrappers.HttpClient;

public interface IHttpClient
{
    public Task<T> PostAsJsonAndGetResultAsync<T>(string? uri, object parameters, JsonSerializerOptions? options = null, CancellationToken token = default);
    public Task PostAsJsonAsync(string? uri, object parameters, JsonSerializerOptions? options = null, CancellationToken token = default);
    public IHttpClientResponse Send(IHttpClientRequest request);
}