using System.Net;
using Shared.ApiControllers.Results;

namespace Servers.Results;

public class TextResult(object result) : IActionResult
{
    public string? Result { get; set; } = result.ToString();
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
}