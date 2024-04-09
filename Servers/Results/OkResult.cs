using System.Net;
using Shared.ApiControllers.Results;

namespace Servers.Results;

public class OkResult : IActionResult
{
    public string? Result { get; set; }
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
}