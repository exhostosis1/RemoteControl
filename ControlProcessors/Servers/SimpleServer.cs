using Shared;
using Shared.Config;
using Shared.ControlProcessor;
using Shared.DataObjects.Http;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using Shared.Server;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Servers;

public class SimpleServer: ServerProcessor
{
    private readonly ILogger<SimpleServer> _logger;
    
    private readonly IListener<HttpContext> _wrapper;
    private readonly AbstractMiddleware<HttpContext> _middleware;

    private CancellationTokenSource? _cst;
    private readonly Progress<bool> _progress;

    private readonly TaskFactory _factory = new();

    public SimpleServer(IListener<HttpContext> wrapper, AbstractMiddleware<HttpContext> middleware, ILogger<SimpleServer> logger, ServerConfig? config = null): base(config)
    {
        _logger = logger;
        _wrapper = wrapper;
        _middleware = middleware;
        _progress = new Progress<bool>(status =>
        {
            Working = status;
            StatusObservers.ForEach(x => x.OnNext(status));
        });
    }

    protected override void StartInternal(ServerConfig config)
    {
        if (_wrapper.IsListening)
        {
            Stop();
        }

        var param = new StartParameters(config.Uri.ToString());

        try
        {
            _wrapper.StartListen(param);
        }
        catch (HttpListenerException e)
        {
            if (e.Message.Contains("Failed to listen"))
            {
                _logger.LogError($"{config.Uri} is already registered");
                return;
            }

            if (!Utils.GetCurrentIPs().Contains(config.Uri.Host))
            {
                _logger.LogError($"{config.Uri.Host} is currently unavailable");
                return;
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw;

            _logger.LogWarn("Trying to add listening permissions to user");

            var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var translatedValue = sid.Translate(typeof(NTAccount)).Value;
            var command =
                $"netsh http add urlacl url={config.Uri} user={translatedValue}";

            Utils.RunWindowsCommandAsAdmin(command);

            _wrapper.StartListen(param);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return;
        }

        _cst = new CancellationTokenSource();

        _factory.StartNew(async () => await ProcessRequestAsync(_progress, _cst.Token),
            TaskCreationOptions.LongRunning);

        _logger.LogInfo($"Started listening on {param.Uri}");
    }

    private async Task ProcessRequestAsync(IProgress<bool> progress, CancellationToken token)
    {
        progress.Report(true);
        while (!token.IsCancellationRequested)
        {
            try
            {
                var context = await _wrapper.GetContextAsync(token);
                token.ThrowIfCancellationRequested();

                _middleware.ProcessRequest(context);

                context.Response.Close();
            }
            catch (Exception e) when (e is OperationCanceledException or TaskCanceledException or ObjectDisposedException)
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

    public override void Stop()
    {
        if (_wrapper.IsListening)
        {
            _cst?.Cancel();
            _cst?.Dispose();
            _wrapper.StopListen();
        }

        _logger.LogInfo("Stopped listening");
    }
}