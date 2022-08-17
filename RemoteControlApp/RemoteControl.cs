using Shared.Interfaces;
using Shared.Interfaces.Web;
using System.Reflection;

namespace RemoteControlApp
{
    public class RemoteControl: IRemoteControlApp
    {
        private record MiddlewareConfigItem(Type? Type, object?[] Parameters, Action<IContext, HttpEventHandler>? Action);

        private readonly IListener _listener;
        private readonly List<MiddlewareConfigItem> _middlewareConfig = new();

        public RemoteControl(IListener listener)
        {
            _listener = listener;
        }

        public IRemoteControlApp UseMiddleware<T>(params object?[] optionalParameters) where T: IMiddleware 
        {
            _middlewareConfig.Add(new MiddlewareConfigItem(typeof(T), optionalParameters, null));
            return this;
        }

        public IRemoteControlApp Use(Action<IContext, HttpEventHandler> middlewareMethod)
        {
            _middlewareConfig.Add(new MiddlewareConfigItem(null, Array.Empty<object?>(), middlewareMethod));
            return this;
        }

        public IRemoteControlApp Build()
        {
            HttpEventHandler next = _ => { };

            for (var i = _middlewareConfig.Count - 1; i >= 0; i--)
            {
                if (_middlewareConfig[i].Type != null)
                {
                    var constructor = _middlewareConfig[i].Type!
                        .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                        .MinBy(x => x.GetParameters().Length);

                    if (_middlewareConfig[i].Parameters.Length + 1 != constructor?.GetParameters().Length)
                        throw new Exception($"Cannot find suitable constructor for type {_middlewareConfig[i].Type}");

                    var middleware =
                        (IMiddleware) constructor.Invoke(new[] { next }.Concat(_middlewareConfig[i].Parameters)
                            .ToArray());

                    next = middleware.ProcessRequest;
                }
                else
                {
                    var middleware = new PlaceholderMiddleware(next, _middlewareConfig[i].Action!);

                    next = middleware.ProcessRequest;
                }
            }

            _listener.OnRequest += next;

            return this;
        }

        public void Start(Uri uri)
        {
            _listener.StartListen(uri.ToString());
        }

        public void Stop() => _listener.StopListen();

        public string? GetUiListeningUri() => _listener.ListeningUris.FirstOrDefault();
        public string? GetApiListeningUri() => _listener.ListeningUris.FirstOrDefault();

        public bool IsUiListening => _listener.IsListening;
        public bool IsApiListening => _listener.IsListening;
    }
}