using System.Net;

namespace Shared.Controllers.Results;

public class StringResult : IActionResult
{
    public StringResult(object result)
    {
        Result = result.ToString();
        StatusCode = HttpStatusCode.OK;
    }

    public string? Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}