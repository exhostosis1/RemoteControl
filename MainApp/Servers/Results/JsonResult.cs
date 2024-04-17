using System.Text.Json;
using MainApp.Servers.DataObjects;

namespace MainApp.Servers.Results;

internal class JsonResult(object result) : IActionResult
{
    public string? Result { get; set; } = JsonSerializer.Serialize(result);
    public RequestStatus StatusCode { get; set; } = RequestStatus.Json;
}