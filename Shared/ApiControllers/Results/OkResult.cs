using System.Net;

namespace Shared.Controllers.Results;

public class OkResult : IActionResult
{
    public OkResult()
    {
        StatusCode = HttpStatusCode.OK;
    }

    public string? Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}