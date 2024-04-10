using Servers.DataObjects;

namespace Servers.Results;

public class TextResult(object result) : IActionResult
{
    public string? Result { get; set; } = result.ToString();
    public RequestStatus StatusCode { get; set; } = RequestStatus.Text;
}