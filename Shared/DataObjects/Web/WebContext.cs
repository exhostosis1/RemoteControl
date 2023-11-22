namespace Shared.DataObjects.Web;

public class WebContext(WebContextRequest request, WebContextResponse response) : IContext
{
    public IRequest Request => WebRequest;
    public IResponse Response => WebResponse;

    public WebContextRequest WebRequest { get; set; } = request;
    public WebContextResponse WebResponse { get; set; } = response;
}