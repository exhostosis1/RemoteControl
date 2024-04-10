using Servers.DataObjects;

namespace Servers.Results;

public class OkResult : IActionResult
{
    public string? Result { get; set; }
    public RequestStatus StatusCode { get; set; } = RequestStatus.Ok;
}