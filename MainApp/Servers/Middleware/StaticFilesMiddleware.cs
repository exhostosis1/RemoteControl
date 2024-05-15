﻿using MainApp.Servers.DataObjects;
using Microsoft.Extensions.Logging;

namespace MainApp.Servers.Middleware;

internal class StaticFilesMiddleware(ILogger logger, string directory = "www") : IMiddleware
{
    private readonly string _contentFolder = Path.Combine(AppContext.BaseDirectory, directory);


    
    public Task ProcessRequestAsync(RequestContext context, RequestDelegate _)
    {
        var uriPath = context.Request;

        logger.LogInformation("Processing file request {uriPath}", uriPath);

        if (uriPath.Contains(".."))
        {
            context.Status = RequestStatus.NotFound;
            return Task.CompletedTask;
        }

        var path = Path.Combine(_contentFolder, uriPath.Replace("/", "").Replace("\\", ""));

        if (string.IsNullOrEmpty(uriPath) || uriPath == "/")
        {
            path = Path.Combine(path, "index.html");
        }

        if (File.Exists(path))
        {
            context.Status = RequestStatus.File;
            context.Reply = path;
        }
        else
        {
            context.Status = RequestStatus.NotFound;
        }

        return Task.CompletedTask;
    }
}