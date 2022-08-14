using System.Net;
using RemoteControl.App.Interfaces.Web;
using RemoteControl.App.Utility;
using RemoteControl.App.Web.DataObjects;

namespace RemoteControl.App.Web.Listeners
{
    public class GenericListener: IListener
    {
        private HttpListener _listener = new();
        public bool IsListening => _listener.IsListening;
        public IReadOnlyCollection<string> ListeningUris => _listener.Prefixes.ToList();

        public event HttpEventHandler? OnRequest;

        private readonly TaskFactory _factory = new();

        public void StartListen(string url)
        {
            try
            {
                if (_listener.IsListening)
                    _listener.Stop();
            }
            catch (ObjectDisposedException)
            {
                _listener = new HttpListener();
            }

            _listener.Prefixes.Clear();

            _listener.Prefixes.Add(url.ToString());

            _listener.Start();

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

                    if(dto.Response.Payload.Length > 0)
                        context.Response.OutputStream.Write(dto.Response.Payload);
                    
                    context.Response.Close();
                }
                catch (ObjectDisposedException)
                {
                    _listener = new HttpListener();
                    return;
                }
                catch
                {
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
