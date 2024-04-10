namespace Servers.DataObjects;

public class RequestContext()
{
    public InputContext Input { get; set; } = new();
    public OutputContext Output { get; set; } = new();
}