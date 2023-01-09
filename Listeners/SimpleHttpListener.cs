using Shared;
using Shared.DataObjects;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Listeners;

public class SimpleHttpListener : IHttpListener
{
    private HttpListener? _listener;
    public bool IsListening => _listener?.IsListening ?? false;
    public IReadOnlyCollection<Uri> ListeningUris => _listener?.Prefixes.Select(x => new Uri(x)).ToList() ?? new List<Uri>();

    public event HttpEventHandler? OnRequest;
    public event BoolEventHandler? OnStatusChange;

    private readonly ILogger _logger;

    private CancellationTokenSource? _cst;
    private readonly Progress<bool> _progress;

    public SimpleHttpListener(ILogger logger)
    {
        _logger = logger;
        _progress = new Progress<bool>(status => OnStatusChange?.Invoke(status));
    }

    public void StartListen(Uri url)
    {
        if (_listener?.IsListening ?? false)
        {
            StopListen();
        }

        _listener = new HttpListener();
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
        ProcessRequest(_progress, _cst.Token);
#pragma warning restore CS4014

        _logger.LogInfo($"Started listening on {url}");
    }

    private async Task ProcessRequest(IProgress<bool> progress, CancellationToken token)
    {
        progress.Report(true);
        while (!token.IsCancellationRequested)
        {
            try
            {
                var context = await (_listener?.GetContextAsync() ?? throw new Exception("Listener is null"));
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
            catch (Exception e) when (e is OperationCanceledException or TaskCanceledException or ObjectDisposedException or HttpListenerException)
            {
                break;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                break;
            }
        }

        progress.Report(false);
    }

    public void StopListen()
    {
        if (_listener?.IsListening ?? false)
        {
            _cst?.Cancel();
            _cst?.Dispose();
            _listener.Stop();
            _listener = null;
        }

        _logger.LogInfo("Stopped listening");
    }
}