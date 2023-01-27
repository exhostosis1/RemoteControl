namespace Shared.DataObjects.Web;

public class WebContext : IContext
{
    public IRequest Request => WebRequest;
    public IResponse Response => WebResponse;

    public WebContextRequest WebRequest { get; set; }
    public WebContextResponse WebResponse { get; set; }


    public WebContext(WebContextRequest request, WebContextResponse response)
    {
        WebRequest = request;
        WebResponse = response;
    }
}