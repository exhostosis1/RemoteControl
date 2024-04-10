using Microsoft.Extensions.Logging;
using Servers.DataObjects;
using System.Net;
using System.Net.Sockets;

namespace Servers.Listeners;

public class SimpleHttpListener(ILogger logger) : IListener
{
    private HttpListener _listener = new();
    private HttpListenerContext? _context;

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
        if (_context == null) return;

        _context.Response.StatusCode = (int)context.Output.StatusCode;
        _context.Response.ContentType = context.Output.ContentType;

        if (context.Output.Payload.Length > 0)
            _context.Response.OutputStream.Write(context.Output.Payload);

        _context.Response.Close();
    }

    public async Task<RequestContext> GetContextAsync(CancellationToken token = default)
    {
        _context = await _listener.GetContextAsync();

        var result = new RequestContext
        {
            Input = new InputContext
            {
                Path = _context.Request.RawUrl ?? ""
            }
        };

        return result;
    }
}