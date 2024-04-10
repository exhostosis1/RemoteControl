using Servers.DataObjects;

namespace Servers.Results;

public class ErrorResult(string? errorMessage) : IActionResult
{
    public string? Result { get; set; } = errorMessage;
    public RequestStatus StatusCode { get; set; } = RequestStatus.Error;
}