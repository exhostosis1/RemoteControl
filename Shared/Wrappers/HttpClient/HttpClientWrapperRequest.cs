using System;
using System.Net.Http;
using Shared.Listeners;

namespace Shared.Wrappers.HttpClient;

public class HttpClientWrapperRequest : IHttpClientRequest
{
    public HttpMethod Method { get; set; }
    public string RequestUri { get; set; }
    public string Content { get; set; }
    public IHttpClientRequest New(HttpMethod method, string requestUri)
    {
        throw new NotImplementedException();
    }
}