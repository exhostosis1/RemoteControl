using MainApp.Servers.DataObjects;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;

namespace MainApp.Servers.Listeners;

internal class SimpleHttpListener(ILogger logger) : IListener
{
    private HttpListener _listener = new();

    public bool IsListening => _listener.IsListening;
    public event PropertyChangedEventHandler? PropertyChanged;
    
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
            var available = currentIps.Contains(new Uri(param.Uri).Host);

            if (!available)
            {
                logger.LogError("{ip} is currently unavailable", param.Uri);
                return;
            }

            logger.LogWarning("Trying to add listening permissions to user");

            var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var translatedValue = sid.Translate(typeof(NTAccount)).Value;

            var command = $"netsh http add urlacl url={param.Uri} user={translatedValue}";

            AppHost.RunWindowsCommand(command, true);

            StartListen(param);
        }
        catch (ObjectDisposedException)
        {
            _listener = new();
            StartListen(param);
        }

        logger.LogInformation("Http listener started listening on {uri}", param.Uri);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsListening)));
    }

    public void StopListen()
    {
        if (_listener.IsListening)
            _listener.Stop();

        logger.LogInformation("Http listener stopped");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsListening)));
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