using Shared.Interfaces.Web;

namespace RemoteControlApp.Web.Middleware
{
    public abstract class BaseMiddleware: IMiddleware
    {
        public IMiddleware? Next { get; set; }

        public IMiddleware Attach(IMiddleware next)
        {
            if (Next == null) Next = next;
            else
            {
                var localnext = Next;

                while (localnext.Next != null)
                {
                    localnext = localnext.Next;
                }

                localnext.Next = next;
            }

            return this;
        }

        public void ProcessRequest(IContext context)
        {
            ProcessRequestInternal(context);

            Next?.ProcessRequest(context);
        }

        protected abstract void ProcessRequestInternal(IContext context);
    }
}
