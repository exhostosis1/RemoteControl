namespace MainApp.Servers.DataObjects;

internal abstract class RequestContext
{
    public string Request { get; set; } = "";
    public RequestStatus Status { get; set; } = RequestStatus.Ok;
    public string Reply { get; set; } = "";

    public abstract void Close();
}