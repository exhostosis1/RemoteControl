using Shared;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Shared.DataObjects.Http;

namespace Listeners;

public class SimpleHttpListener : IHttpListener
{
    public bool IsListening => _wrapper.IsListening;

    public event EventHandler<Context>? OnRequest;
    public event EventHandler<bool>? OnStatusChange;

    private readonly ILogger<SimpleHttpListener> _logger;
    private readonly IHttpListenerWrapper _wrapper;

    private CancellationTokenSource? _cst;
    private readonly Progress<bool> _progress;

    private readonly TaskFactory _factory = new();

    public SimpleHttpListener(IHttpListenerWrapper wrapper, ILogger<SimpleHttpListener> logger)
    {
        _logger = logger;
        _wrapper = wrapper;
        _progress = new Progress<bool>(status => OnStatusChange?.Invoke(this, status));
    }

    public void StartListen(Uri url)
    {
        if (_wrapper.IsListening)
        {
            StopListen();
        }

        try
        {
            _wrapper.Start(url);
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

            _wrapper.Start(url);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return;
        }

        _cst = new CancellationTokenSource();

        _factory.StartNew(async () => await ProcessRequestAsync(_progress, _cst.Token),
            TaskCreationOptions.LongRunning);

        _logger.LogInfo($"Started listening on {url}");
    }

    private async Task ProcessRequestAsync(IProgress<bool> progress, CancellationToken token)
    {
        progress.Report(true);
        while (!token.IsCancellationRequested)
        {
            try
            {
                var context = await _wrapper.GetContextAsync();
                token.ThrowIfCancellationRequested();

                OnRequest?.Invoke(this, context);

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
        if (_wrapper.IsListening)
        {
            _cst?.Cancel();
            _cst?.Dispose();
            _wrapper.Stop();
        }

        _logger.LogInfo("Stopped listening");
    }
}