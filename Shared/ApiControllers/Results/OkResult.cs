using System.Net;

namespace Shared.ApiControllers.Results;

public class OkResult : IActionResult
{
    public string? Result { get; set; }
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
}