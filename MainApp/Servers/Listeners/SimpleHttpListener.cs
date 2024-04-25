using System.Net;
using System.Net.Sockets;
using System.Text;
using MainApp.Servers.DataObjects;
using Microsoft.Extensions.Logging;

namespace MainApp.Servers.Listeners;

internal class SimpleHttpListener(ILogger logger) : IListener
{
    private HttpListener _listener = new();

    public bool IsListening => _listener.IsListening;

    private static IEnumerable<string> GetCurrentIPs() =>
        Dns.GetHostAddresses(Dns.GetHostName(), AddressFamily.InterNetwork).Select(x => x.ToString());

    public void StartListen(StartParameters param)
    {
        if (_listener.IsListening)
        {
            _listener.Stop();
        }

        _listener = new HttpListener();
        _listener.Prefixes.Add(param.Uri);

        if (_listener.IsListening) return;

        try
        {
            _listener.Start();
        }
        catch (HttpListenerException e)
        {
            if (e.Message.Contains("Failed to listen"))
            {
                logger.LogError("{message}", e.Message);
                return;
            }

            var currentIps = GetCurrentIPs();

            
            var unavailableIps = _listener.Prefixes.Where(x => !currentIps.Contains(new Uri(x).Host)).ToList();

            if (unavailableIps.Count == 0) throw;

            logger.LogError("{ip} is currently unavailable", string.Join(';', unavailableIps));
            return;
        }

        logger.LogInformation("Http listener started listening on {uri}", param.Uri);
    }

    public void StopListen()
    {
        if (_listener.IsListening)
            _listener.Stop();

        logger.LogInformation("Http listener stopped");
    }

    public void CloseContext(RequestContext context)
    {
        if (context.OriginalRequest is not HttpListenerContext original) return;

        switch (context.Status)
        {
            case RequestStatus.Ok:
                original.Response.StatusCode = (int)HttpStatusCode.OK;
                break;
            case RequestStatus.Text:
                original.Response.StatusCode = (int)HttpStatusCode.OK;
                original.Response.ContentType = "text/plain";
                original.Response.OutputStream.Write(Encoding.UTF8.GetBytes(context.Reply));
                break;
            case RequestStatus.Json:
                original.Response.StatusCode = (int)HttpStatusCode.OK;
                original.Response.ContentType = "application/json";
                original.Response.OutputStream.Write(Encoding.UTF8.GetBytes(context.Reply));
                break;
            case RequestStatus.Error:
                original.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                original.Response.OutputStream.Write(Encoding.UTF8.GetBytes(context.Reply));
                break;
            case RequestStatus.NotFound:
                original.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            case RequestStatus.Custom:
                break;
            default:
                break;
        }

        original.Response.Close();
    }

    public async Task<RequestContext> GetContextAsync(CancellationToken token = default)
    {
        var context = await _listener.GetContextAsync();

        var result = new RequestContext
        {
            Path = context.Request.RawUrl ?? "",
            OriginalRequest = context
        };

        return result;
    }
}