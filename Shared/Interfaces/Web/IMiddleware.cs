namespace Shared.Interfaces.Web
{
    public interface IMiddleware
    {
        public void ProcessRequest(IContext context);
    }
}