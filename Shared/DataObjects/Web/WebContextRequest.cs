namespace Shared.DataObjects.Web;

public class WebContextRequest(string path) : IRequest
{
    public string Path { get; set; } = path;
}