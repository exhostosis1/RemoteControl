using System.Net;

namespace Shared.Controllers.Results;

public interface IActionResult
{
    public string? Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}