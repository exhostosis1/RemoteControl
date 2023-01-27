using System.Threading.Tasks;

namespace Shared.Wrappers.HttpClient;

public interface IHttpClientResponseContent
{
    public Task<T?> ReadFromJsonAsync<T>();
}