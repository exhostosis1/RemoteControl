using Shared.Interfaces.Web;

namespace RemoteControlApp.Middleware
{
    internal class PlaceholderMiddleware : IMiddleware
    {
        private readonly Action<IContext, HttpEventHandler> _action;
        private readonly HttpEventHandler _next;

        public PlaceholderMiddleware(HttpEventHandler next, Action<IContext, HttpEventHandler> action)
        {
            _action = action;
            _next = next;
        }

        public void ProcessRequest(IContext context)
        {
            _action(context, _next);
        }
    }
}
