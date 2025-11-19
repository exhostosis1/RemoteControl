using MainApp.Workers.DataObjects;

namespace MainApp.Workers.Results;

internal class TextResult(object result) : IActionResult
{
    public string? Result { get; set; } = result.ToString();
    public RequestStatus StatusCode { get; set; } = RequestStatus.Text;
}