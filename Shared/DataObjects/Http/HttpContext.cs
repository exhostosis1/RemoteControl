using System;
using System.Net;

namespace Shared.DataObjects.Http;

public class HttpContext: IContext
{
    public Request Request { get; set; }
    public Response Response { get; set; } = new();

    public HttpContext(string path) => Request = new Request(path);

    public HttpContext(HttpListenerContext context)
    {
        Request = new Request(context.Request.RawUrl ?? throw new ArgumentNullException(nameof(context), "Path is null"));
        Response = new Response(context.Response);
    }
}