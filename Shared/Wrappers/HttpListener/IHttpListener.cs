using System.Threading.Tasks;
using Shared.DataObjects.Web;

namespace Shared.Wrappers.HttpListener;

public interface IHttpListener
{
    public bool IsListening { get; }
    public void Start();
    public void Stop();
    public IPrefixesCollection Prefixes { get; }
    public WebContext GetContext();
    public Task<WebContext> GetContextAsync();
    public void GetNew();
}