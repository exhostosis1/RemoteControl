using RemoteControl.App.Interfaces.Web;

namespace RemoteControl.App.Web.DataObjects
{
    internal class Context: IContext
    {
        public IRequest Request { get; set; }
        public IResponse Response { get; set; } = new Response();

        internal Context(string path) => Request = new Request(path);
    }
}
