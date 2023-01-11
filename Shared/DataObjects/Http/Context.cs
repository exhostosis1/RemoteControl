using System;
using System.Net;

namespace Shared.DataObjects.Http;

public class Context
{
    public Request Request { get; set; }
    public Response Response { get; set; } = new();

    public Context(string path) => Request = new Request(path);

    public Context(HttpListenerContext context)
    {
        Request = new Request(context.Request.RawUrl ?? throw new ArgumentNullException(nameof(context), "Path is null"));
        Response = new Response(context.Response);
    }
}