using Shared;
using Shared.DataObjects;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.Server.Interfaces;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Listeners;

public class GenericListener : IListener
{
    private HttpListener _listener = new();
    public bool IsListening => _listener.IsListening;
    public IReadOnlyCollection<Uri> ListeningUris => _listener.Prefixes.Select(x => new Uri(x)).ToList();

    public event HttpEventHandler? OnRequest;

    private readonly ILogger _logger;

    public GenericListener(ILogger logger)
    {
        _logger = logger;
    }

    private CancellationTokenSource? _cst;

    public void StartListen(Uri url)
    {
        try
        {
            if (_listener.IsListening)
                _listener.Stop();
        }
        catch(ObjectDisposedException)
        {

        }
        finally
        {
            _listener = new HttpListener();
        }

        _listener.Prefixes.Add(url.ToString());

        try
        {
            _listener.Start();
        }
        catch (HttpListenerException e)
        {
            if(e.Message.Contains("Failed to listen"))
            {
                _logger.LogError($"{url} is already registered");
                return;
            }

            if(!Utils.GetCurrentIPs().Contains(url.Host))
            {
                _logger.LogError($"{url.Host} is currently unavailable");
                return;
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw;

            _logger.LogWarn("Trying to add listening permissions to user");

            var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var translatedValue = sid.Translate(typeof(NTAccount)).Value;
            var command =
                $"netsh http add urlacl url={url} user={translatedValue}";

            Utils.RunWindowsCommandAsAdmin(command);

            _listener.Start();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return;
        }

        _cst = new CancellationTokenSource();
#pragma warning disable CS4014
        ProcessRequest(_cst.Token);
#pragma warning restore CS4014

        _logger.LogInfo($"Started listening on {url}");
    }

    private async Task ProcessRequest(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                token.ThrowIfCancellationRequested();

                var path = context.Request.RawUrl;
                if (path == null) return;

                var dto = new Context(path);

                OnRequest?.Invoke(dto);

                context.Response.StatusCode = (int)dto.Response.StatusCode;
                context.Response.ContentType = dto.Response.ContentType;

                if (dto.Response.Payload.Length > 0)
                    context.Response.OutputStream.Write(dto.Response.Payload);

                context.Response.Close();
            }
            catch (OperationCanceledException)
            {
                if(_listener.IsListening)
                    _listener.Stop();
                return;
            }
            catch (ObjectDisposedException)
            {
                _listener = new HttpListener();
                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);

                if (!_listener.IsListening)
                    return;
            }
        }
    }

    public void StopListen()
    {
        if (_listener.IsListening)
        {
            _cst?.Cancel();
            _listener.Stop();
        }

        _logger.LogInfo("Stopped listening");
    }
}