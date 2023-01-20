using Shared;
using Shared.Listeners;
using Shared.Logging.Interfaces;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Shared.DataObjects.Http;

namespace Listeners;

public class SimpleHttpListener : IListener<HttpContext>
{
    public ListenerState State { get; private set; }

    private readonly ILogger<SimpleHttpListener> _logger;
    private readonly IHttpListenerWrapper _wrapper;

    private CancellationTokenSource? _cst;
    private readonly Progress<bool> _progress;

    private readonly TaskFactory _factory = new();

    private readonly List<IObserver<bool>> _statusObservers = new();
    private readonly List<IObserver<HttpContext>> _requestObservers = new();

    public SimpleHttpListener(IHttpListenerWrapper wrapper, ILogger<SimpleHttpListener> logger)
    {
        _logger = logger;
        _wrapper = wrapper;
        State = new ListenerState();
        _progress = new Progress<bool>(status =>
        {
            State.Listening = status;
            _statusObservers.ForEach(x => x.OnNext(status));
        });
    }

    public void StartListen(StartParameters param)
    {
        if (_wrapper.IsListening)
        {
            StopListen();
        }

        try
        {
            _wrapper.Start(param.Uri);
        }
        catch (HttpListenerException e)
        {
            if(e.Message.Contains("Failed to listen"))
            {
                _logger.LogError($"{param.Uri} is already registered");
                return;
            }

            if(!Utils.GetCurrentIPs().Contains(param.Uri.Host))
            {
                _logger.LogError($"{param.Uri.Host} is currently unavailable");
                return;
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw;

            _logger.LogWarn("Trying to add listening permissions to user");

            var sid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            var translatedValue = sid.Translate(typeof(NTAccount)).Value;
            var command =
                $"netsh http add urlacl url={param.Uri} user={translatedValue}";

            Utils.RunWindowsCommandAsAdmin(command);

            _wrapper.Start(param.Uri);
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
                var context = await _wrapper.GetContextAsync();
                token.ThrowIfCancellationRequested();

                _requestObservers.ForEach(x => x.OnNext(context));

                context.Response.Close();
            }
            catch (Exception e) when (e is OperationCanceledException or TaskCanceledException or ObjectDisposedException or HttpListenerException)
            {
                break;
            }
            catch (Exception e)
            {
                _requestObservers.ForEach(x => x.OnError(e));
                _logger.LogError(e.Message);
                break;
            }
        }

        _requestObservers.ForEach(x => x.OnCompleted());
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

    public IDisposable Subscribe(IObserver<bool> observer)
    {
        _statusObservers.Add(observer);
        return new Unsubscriber<bool>(_statusObservers, observer);
    }

    public IDisposable Subscribe(IObserver<HttpContext> observer)
    {
        _requestObservers.Add(observer);
        return new Unsubscriber<HttpContext>(_requestObservers, observer);
    }
}