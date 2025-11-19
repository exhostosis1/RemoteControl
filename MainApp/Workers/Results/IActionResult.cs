using MainApp.Workers.DataObjects;

namespace MainApp.Workers.Results;

internal interface IActionResult
{
    public string? Result { get; set; }
    public RequestStatus StatusCode { get; set; }
}