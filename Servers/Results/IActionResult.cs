using System.Net;

namespace Servers.Results;

public interface IActionResult
{
    public string? Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}