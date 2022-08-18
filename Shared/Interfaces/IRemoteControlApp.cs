using Shared.Interfaces.Web;

namespace Shared.Interfaces
{
    public interface IRemoteControlApp
    {
        public void Start(Uri uri);
        public void Stop();
        public string? GetUiListeningUri();
        public string? GetApiListeningUri();
        public bool IsUiListening { get; }
        public bool IsApiListening { get; }

        public IRemoteControlApp UseMiddleware<T>() where T : IMiddleware;
        public IRemoteControlApp Use(Action<IContext, HttpEventHandler> middlewareMethod);
        public IRemoteControlApp Build();
    }
}