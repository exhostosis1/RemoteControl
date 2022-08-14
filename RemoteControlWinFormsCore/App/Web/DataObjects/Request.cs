using RemoteControl.App.Interfaces.Web;

namespace RemoteControl.App.Web.DataObjects
{
    internal class Request: IRequest
    {
        public string Path { get; set; }

        internal Request(string path) => Path = path;
    }
}
