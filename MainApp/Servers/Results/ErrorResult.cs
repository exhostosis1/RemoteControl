using MainApp.Servers.DataObjects;

namespace MainApp.Servers.Results;

internal class ErrorResult(string? errorMessage) : IActionResult
{
    public string? Result { get; set; } = errorMessage;
    public RequestStatus StatusCode { get; set; } = RequestStatus.Error;
}