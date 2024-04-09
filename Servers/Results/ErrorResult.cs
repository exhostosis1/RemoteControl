﻿using System.Net;

namespace Servers.Results;

public class ErrorResult(string? errorMessage) : IActionResult
{
    public string? Result { get; set; } = errorMessage;
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
}