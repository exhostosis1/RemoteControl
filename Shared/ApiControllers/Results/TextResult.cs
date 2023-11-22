using System.Net;

namespace Shared.ApiControllers.Results;

public class TextResult(object result) : IActionResult
{
    public string? Result { get; set; } = result.ToString();
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
}