namespace Shared.Interfaces.Web
{
    public interface IMiddleware
    {
        public IMiddleware? Next { get; set; }
        public IMiddleware First { get; set; }
        public IMiddleware Attach(IMiddleware next);
        public IMiddleware GetFirst();
        public void ProcessRequest(IContext context);
    }
}