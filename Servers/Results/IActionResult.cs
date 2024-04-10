using Servers.DataObjects;

namespace Servers.Results;

public interface IActionResult
{
    public string? Result { get; set; }
    public RequestStatus StatusCode { get; set; }
}