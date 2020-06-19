namespace RemoteControlCore.Interfaces
{
    internal interface IHttpListener: IListener
    {
        event HttpEventHandler OnHttpRequest;
    }
}
