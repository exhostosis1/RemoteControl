namespace RemoteControl.App.Interfaces.Web
{
    public interface IContext
    {
        public IRequest Request { get; }
        public IResponse Response { get; }
    }
}
