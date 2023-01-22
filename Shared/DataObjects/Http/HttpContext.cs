namespace Shared.DataObjects.Http;

public class HttpContext: IContext
{
    public IRequest Request => HttpRequest;
    public IResponse Response => HttpResponse;

    public HttpContextRequest HttpRequest { get; set; }
    public HttpContextResponse HttpResponse { get; set; }


    public HttpContext(HttpContextRequest request, HttpContextResponse response)
    {
        HttpRequest = request;
        HttpResponse = response;
    }
}