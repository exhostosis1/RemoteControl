using Shared.DataObjects.Http;
using System.Threading.Tasks;

namespace Shared.Listeners;

public interface IHttpListener
{
    public bool IsListening { get; }
    public void Start();
    public void Stop();
    public IPrefixesCollection Prefixes { get; }
    public HttpContext GetContext();
    public Task<HttpContext> GetContextAsync();
    public void GetNew();
}