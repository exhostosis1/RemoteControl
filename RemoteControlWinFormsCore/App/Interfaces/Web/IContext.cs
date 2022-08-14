namespace RemoteControl.App.Interfaces.Web
{
    public interface IContext
    {
        public IRequest Request { get; set; }
        public IResponse Response { get; set; }
    }
}
