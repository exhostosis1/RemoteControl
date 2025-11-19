using MainApp.Workers.DataObjects;
using System.Text.Json;

namespace MainApp.Workers.Results;

internal class JsonResult(object result) : IActionResult
{
    public string? Result { get; set; } = JsonSerializer.Serialize(result);
    public RequestStatus StatusCode { get; set; } = RequestStatus.Json;
}