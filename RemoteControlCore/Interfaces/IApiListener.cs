namespace RemoteControlCore.Interfaces
{
    internal interface IApiListener: IListener
    {
        event ApiEventHandler OnApiRequest;
    }
}
