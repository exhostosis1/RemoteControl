namespace Shared.DataObjects.Http;

public class HttpContext: IContext
{
    public Request Request { get; set; }
    public Response Response { get; set; }


    public HttpContext(Request request, Response response)
    {
        Request = request;
        Response = response;
    }
}