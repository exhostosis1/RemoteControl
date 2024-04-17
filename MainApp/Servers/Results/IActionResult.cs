using MainApp.Servers.DataObjects;

namespace MainApp.Servers.Results;

internal interface IActionResult
{
    public string? Result { get; set; }
    public RequestStatus StatusCode { get; set; }
}