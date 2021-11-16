using System.Net;

namespace RemoteControl.Core.Interfaces
{
    internal interface IHttpRequestArgs
    {
        HttpListenerRequest Request { get; }
        HttpListenerResponse Response { get; }
    }
}
