using System.Net;

namespace RemoteControlCore.Interfaces
{
    internal interface IHttpRequestArgs
    {
        HttpListenerRequest Request { get; }
        HttpListenerResponse Response { get; }
    }
}
