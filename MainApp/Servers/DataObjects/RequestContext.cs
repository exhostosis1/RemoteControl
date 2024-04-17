namespace MainApp.Servers.DataObjects;

public class RequestContext
{
    public string Path { get; set; } = "";
    public RequestStatus Status { get; set; } = RequestStatus.Ok;
    public string Reply { get; set; } = "";

    public object? OriginalRequest { get; set; }
}