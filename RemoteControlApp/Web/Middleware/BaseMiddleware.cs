using Shared.Interfaces.Web;

namespace RemoteControlApp.Web.Middleware
{
    public abstract class BaseMiddleware: IMiddleware
    {
        public IMiddleware? Next { get; set; }
        public IMiddleware First { get; set; }

        protected BaseMiddleware()
        {
            First = this;
        }

        public IMiddleware Attach(IMiddleware next)
        {
            Next = next;
            next.First = First;

            return next;
        }

        public IMiddleware GetFirst() => First;

        public void ProcessRequest(IContext context)
        {
            ProcessRequestInternal(context);

            Next?.ProcessRequest(context);
        }

        protected abstract void ProcessRequestInternal(IContext context);
    }
}
