using System.Net;

namespace Shared.ApiControllers.Results;

public class ErrorResult : IActionResult
{
    public ErrorResult(string? errorMessage)
    {
        StatusCode = HttpStatusCode.InternalServerError;
        Result = errorMessage;
    }

    public string? Result { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}