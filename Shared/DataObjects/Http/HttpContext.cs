namespace Shared.DataObjects.Http;

public class HttpContext: IContext
{
    public IRequest Request => HttpRequest;
    public IResponse Response => HttpResponse;

    public Request HttpRequest { get; set; }
    public Response HttpResponse { get; set; }


    public HttpContext(Request request, Response response)
    {
        HttpRequest = request;
        HttpResponse = response;
    }
}