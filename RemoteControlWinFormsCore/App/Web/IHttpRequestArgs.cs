using System.Net;

namespace RemoteControl.App.Web.Interfaces
{
    internal interface IHttpRequestArgs
    {
        HttpListenerRequest Request { get; }
        HttpListenerResponse Response { get; }
    }
}
