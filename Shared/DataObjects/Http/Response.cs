using System;
using System.Net;

namespace Shared.DataObjects.Http;

public abstract class Response: IResponse
{
    public string ContentType { get; set; } = "text/plain";
    public byte[] Payload { get; set; } = Array.Empty<byte>();
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    public abstract void Close();
}