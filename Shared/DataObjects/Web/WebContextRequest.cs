namespace Shared.DataObjects.Web;

public class WebContextRequest : IRequest
{
    public string Path { get; set; }

    public WebContextRequest(string path) => Path = path;
}