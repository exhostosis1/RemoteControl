using System;
using System.Net;

namespace Shared.DataObjects.Http;

public class Response
{
    private readonly HttpListenerResponse? _response;

    public Response(HttpListenerResponse response)
    {
        _response = response;
    }

    public Response()
    {

    }

    public string ContentType { get; set; } = "text/plain";
    public byte[] Payload { get; set; } = Array.Empty<byte>();
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    public void Close()
    {
        if (_response != null)
        {
            _response.StatusCode = (int)StatusCode;
            _response.ContentType = ContentType;

            if(Payload.Length > 0)
                _response.OutputStream.Write(Payload);

            _response.Close();
        }
    }
}