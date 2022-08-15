namespace Shared.Interfaces.Web
{
    public interface IMiddleware
    {
        public IMiddleware? Next { get; set; }
        public IMiddleware Attach(IMiddleware next);
        public void ProcessRequest(IContext context);
    }
}