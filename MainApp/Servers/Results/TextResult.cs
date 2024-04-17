using MainApp.Servers.DataObjects;

namespace MainApp.Servers.Results;

internal class TextResult(object result) : IActionResult
{
    public string? Result { get; set; } = result.ToString();
    public RequestStatus StatusCode { get; set; } = RequestStatus.Text;
}