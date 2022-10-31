using Shared;
using Shared.DataObjects;
using Shared.Logging.Interfaces;
using Shared.Server;
using Shared.Server.Interfaces;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Listeners
{
    public class GenericListener : IListener
    {
        private HttpListener _listener = new();
        public bool IsListening => _listener.IsListening;
        public IReadOnlyCollection<string> ListeningUris => _listener.Prefixes.ToList();

        public event HttpEventHandler? OnRequest;

        private readonly TaskFactory _factory = new();

        private readonly ILogger _logger;

        public GenericListener(ILogger logger)
        {
            _logger = logger;
        }

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
                    _logger.LogError($"{url.ToString} is already registered");

                    return;
                }

                var ips = Utils.GetCurrentIPs();

                if(!Utils.GetCurrentIPs().Contains(url.Host))
                {
                    _logger.LogError($"{url.Host} is currently unavailable");

                    return;
                }

                _logger.LogWarn("Trying to add listening permissions to user");

                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    throw;

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
                throw;
            }

            _logger.LogInfo($"started listening on {url}");

            _factory.StartNew(ProcessRequest, TaskCreationOptions.LongRunning);
        }

        private async Task ProcessRequest()
        {
            while (true)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
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
                _listener.Stop();
            }
        }
    }
}
