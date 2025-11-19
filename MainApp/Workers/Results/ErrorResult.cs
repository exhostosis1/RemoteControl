using MainApp.Workers.DataObjects;

namespace MainApp.Workers.Results;

internal class ErrorResult(string? errorMessage) : IActionResult
{
    public string? Result { get; set; } = errorMessage;
    public RequestStatus StatusCode { get; set; } = RequestStatus.Error;
}