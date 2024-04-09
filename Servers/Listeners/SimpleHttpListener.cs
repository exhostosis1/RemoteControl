using Microsoft.Extensions.Logging;
using Servers.DataObjects;
using Servers.DataObjects.Web;
using System.Net;
using System.Net.Sockets;

namespace Servers.Listeners;

public class SimpleHttpListener(ILogger logger) : IListener
{
    private class LocalResponse(HttpListenerResponse response) : WebContextResponse
    {
        public override void Close()
        {
            response.StatusCode = (int)StatusCode;
            response.ContentType = ContentType;

            if (Payload.Length > 0)
                response.OutputStream.Write(Payload);

            response.Close();
        }
    }

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

            if (unavailableIps.Count > 0)
            {
                logger.LogError("{ip} is currently unavailable", string.Join(';', unavailableIps));
                return;
            }

            throw;
        }

        logger.LogInformation("Http listener started listening on {uri}", param.Uri);
    }

    public void StopListen()
    {
        if (_listener.IsListening)
            _listener.Stop();

        logger.LogInformation("Http listener stopped");
    }

    public async Task<IContext> GetContextAsync(CancellationToken token = default)
    {
        var context = await _listener.GetContextAsync();

        return new WebContext(new WebContextRequest(context.Request.RawUrl ?? ""), new LocalResponse(context.Response));
    }
}