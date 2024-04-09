using Microsoft.Extensions.Logging;
using Servers.DataObjects;
using Servers.DataObjects.Web;
using Shared;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;

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

            var currentIps = Utils.GetCurrentIPs();
            var unavailableIps = _listener.Prefixes.Where(x => !currentIps.Contains(new Uri(x).Host)).ToList();

            if (unavailableIps.Count > 0)
            {
                logger.LogError("{ip} is currently unavailable", string.Join(';', unavailableIps));
                return;
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw;

            logger.LogWarning("Trying to add listening permissions to user");

            var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var translatedValue = sid.Translate(typeof(NTAccount)).Value;

            foreach (var prefix in _listener.Prefixes)
            {
                var command = $"netsh http add urlacl url={prefix} user={translatedValue}";

                Utils.RunWindowsCommandAsAdmin(command);
            }

            _listener.Start();
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