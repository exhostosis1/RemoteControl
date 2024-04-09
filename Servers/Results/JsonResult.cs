using System.Net;
using System.Text.Json;

namespace Servers.Results;

public class JsonResult(object result) : IActionResult
{
    public string? Result { get; set; } = JsonSerializer.Serialize(result);
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
}