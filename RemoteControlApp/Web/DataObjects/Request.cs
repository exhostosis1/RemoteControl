using Shared.Interfaces.Web;

namespace RemoteControlApp.Web.DataObjects
{
    internal class Request: IRequest
    {
        public string Path { get; set; }

        internal Request(string path) => Path = path;
    }
}
