using Shared.Interfaces;
using Shared.Interfaces.Web;
using System.Reflection;
using RemoteControlApp.Middleware;

namespace RemoteControlApp
{
    public class RemoteControl: IRemoteControlApp
    {
        private readonly IListener _listener;
        private readonly IContainer _container;
        private readonly List<object> _middlewareConfig = new();

        public RemoteControl(IListener listener, IContainer container)
        {
            _container = container;
            _listener = listener;
        }

        public IRemoteControlApp UseMiddleware<T>() where T: IMiddleware 
        {
            _middlewareConfig.Add(typeof(T));
            return this;
        }

        public IRemoteControlApp Use(Action<IContext, HttpEventHandler> middlewareMethod)
        {
            _middlewareConfig.Add(middlewareMethod);
            return this;
        }

        public IRemoteControlApp Build()
        {
            HttpEventHandler next = _ => { };

            for (var i = _middlewareConfig.Count - 1; i >= 0; i--)
            {
                if (_middlewareConfig[i] is Type type)
                {
                    var constructor = type
                        .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
                        .MinBy(x => x.GetParameters().Length);

                    if (constructor == null)
                        throw new Exception($"Cannot find public constructor for {type}");

                    var parameters = constructor.GetParameters()
                        .Select(x => x.ParameterType == typeof(HttpEventHandler) ? next : _container.Get(x.ParameterType)).ToArray();

                    var middleware = (IMiddleware) constructor.Invoke(parameters);

                    next = middleware.ProcessRequest;
                }
                else
                {
                    var middleware = new PlaceholderMiddleware(next, (Action<IContext, HttpEventHandler>)_middlewareConfig[i]);

                    next = middleware.ProcessRequest;
                }
            }

            _listener.OnRequest += next;

            return this;
        }

        public void Start(Uri uri) => _listener.StartListen(uri.ToString());

        public void Stop() => _listener.StopListen();

        public string? GetUiListeningUri() => _listener.ListeningUris.FirstOrDefault();
        public string? GetApiListeningUri() => _listener.ListeningUris.FirstOrDefault();

        public bool IsUiListening => _listener.IsListening;
        public bool IsApiListening => _listener.IsListening;
    }
}