using Shared.DataObjects.Http;
using Shared.Listeners;
using System.Net;
using System.Threading.Tasks;

namespace Shared.Wrappers;

public class PrefixCollection: IPrefixesCollection
{
    private readonly HttpListenerPrefixCollection _collection;

    public PrefixCollection(HttpListenerPrefixCollection collection)
    {
        _collection = collection;
    }

    public void Add(string prefix) => _collection.Add(prefix);
}

public class HttpListenerWrapper: IHttpListener
{
    class LocalResponse: Response
    {
        private readonly HttpListenerResponse _response;

        public LocalResponse(HttpListenerResponse response)
        {
            _response = response;
        }

        public override void Close()
        {
            _response.StatusCode = (int)StatusCode;
            _response.ContentType = ContentType;
            
            if(Payload.Length > 0)
                _response.OutputStream.Write(Payload);

            _response.Close();
        }
    }

    private HttpListener _listener = new();
    public bool IsListening => _listener.IsListening;
    public IPrefixesCollection Prefixes { get; private set; }

    public HttpListenerWrapper()
    {
        Prefixes = new PrefixCollection(_listener.Prefixes);
    }

    public void Start()
    {
        if (!_listener.IsListening)
            _listener.Start();
    }

    public void Stop()
    {
        if(_listener.IsListening)
            _listener.Stop();
    }

    public void GetNew()
    {
        if(_listener.IsListening)
            _listener.Stop();

        _listener = new HttpListener();
        Prefixes = new PrefixCollection(_listener.Prefixes);
    }

    private HttpContext ConvertContext(HttpListenerContext context)
    {
        var request = new Request(context.Request.RawUrl ?? string.Empty);
        var response = new LocalResponse(context.Response);
        return new HttpContext(request, response);
    }

    public HttpContext GetContext() => ConvertContext(_listener.GetContext());
    public async Task<HttpContext> GetContextAsync() => ConvertContext(await _listener.GetContextAsync());
}