using System.Threading.Tasks;

namespace Shared.Listeners;

public interface IHttpClientResponseContent
{
    public Task<T?> ReadFromJsonAsync<T>();
}