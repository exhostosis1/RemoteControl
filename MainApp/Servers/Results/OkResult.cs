using MainApp.Servers.DataObjects;

namespace MainApp.Servers.Results;

internal class OkResult : IActionResult
{
    public string? Result { get; set; }
    public RequestStatus StatusCode { get; set; } = RequestStatus.Ok;
}