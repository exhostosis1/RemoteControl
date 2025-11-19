using MainApp.Workers.DataObjects;

namespace MainApp.Workers.Results;

internal class OkResult : IActionResult
{
    public string? Result { get; set; }
    public RequestStatus StatusCode { get; set; } = RequestStatus.Ok;
}