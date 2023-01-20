using Shared.DataObjects.Http;
using System;
using System.Threading.Tasks;

namespace Shared.Listeners;

public interface IHttpListenerWrapper
{
    public void Start(Uri uri);
    public void Stop();

    public bool IsListening { get; }

    public Task<HttpContext> GetContextAsync();
}