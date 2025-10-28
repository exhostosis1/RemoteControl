using MainApp.Servers.DataObjects;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace MainApp.Servers.Listeners;

internal class SimpleHttpListener(ILogger logger) : IListener
{
    private HttpListener? _listener;

    public bool IsListening
    {
        get
        {
            try
            {
                return _listener?.IsListening ?? false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void StartListen(StartParameters param)
    {
        try
        {
            if (_listener?.IsListening ?? false)
            {
                _listener.Stop();
            }
        }
        catch
        {
            // ignored
        }

        _listener = new HttpListener();
        _listener.Prefixes.Add(param.Uri);

        try
        {
            _listener.Start();
        }
        catch (HttpListenerException e)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError("{message}", e.Message);
            }
            throw;
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Http listener started listening on {uri}", param.Uri);
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsListening)));
    }

    public void StopListen()
    {
        try
        {
            if (!(_listener?.IsListening ?? false)) return;

            _listener.Stop();
            _listener = null;

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Http listener stopped");
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsListening)));
        }
        catch
        {
            // ignored
        }
    }

    public async Task<RequestContext> GetContextAsync(CancellationToken token = default)
    {
        HttpListenerContext context;

        try
        {
            context = await (_listener?.GetContextAsync() ?? throw new NullReferenceException("HttpListener is stopped"));

            token.ThrowIfCancellationRequested();
        }
        catch (Exception e)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError("{message}", e.Message);
            }
            throw;
        }

        var result = new HttpRequestContext(context.Response)
        {
            Request = context.Request.RawUrl ?? "",
        };

        return result;
    }

    private class HttpRequestContext(HttpListenerResponse response) : RequestContext
    {
        private static readonly Dictionary<string, string> ContentTypes = new()
        {
            { ".html", "text/html" },
            { ".htm", "text/html" },
            { ".ico", "image/x-icon" },
            { ".js", "text/javascript" },
            { ".mjs", "text/javascript" },
            { ".css", "text/css" }
        };

        public override void Close()
        {
            switch (Status)
            {
                case RequestStatus.Ok:
                    response.StatusCode = (int)HttpStatusCode.OK;
                    break;
                case RequestStatus.Text:
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "text/plain";
                    response.OutputStream.Write(Encoding.UTF8.GetBytes(Reply));
                    break;
                case RequestStatus.Json:
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.ContentType = "application/json";
                    response.OutputStream.Write(Encoding.UTF8.GetBytes(Reply));
                    break;
                case RequestStatus.Error:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.OutputStream.Write(Encoding.UTF8.GetBytes(Reply));
                    break;
                case RequestStatus.NotFound:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case RequestStatus.File:
                    response.ContentType = ContentTypes.GetValueOrDefault(Path.GetExtension(Reply), "text/plain");
                    response.OutputStream.Write(File.ReadAllBytes(Reply));
                    response.StatusCode = (int)HttpStatusCode.OK;
                    break;
            }

            response.Close();
        }
    }
}