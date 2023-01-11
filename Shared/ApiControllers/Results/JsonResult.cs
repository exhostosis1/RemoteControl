using System.Net;
using System.Text.Json;

namespace Shared.ApiControllers.Results;

public class JsonResult : IActionResult
{
    public JsonResult(object result)
    {
        Result = JsonSerializer.Serialize(result);
        StatusCode = HttpStatusCode.OK;
    }

    public string? Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}