namespace RemoteControl.App.Interfaces.Web
{
    public interface IMiddleware
    {
        public void ProcessRequest(IContext context);
    }
}