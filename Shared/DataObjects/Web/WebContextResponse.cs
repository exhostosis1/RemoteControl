﻿using System;
using System.Net;

namespace Shared.DataObjects.Web;

public abstract class WebContextResponse : IResponse
{
    public string ContentType { get; set; } = "text/plain";
    public byte[] Payload { get; set; } = Array.Empty<byte>();
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    public abstract void Close();
}