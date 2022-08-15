using Shared.Interfaces.Web;

namespace Shared.DataObjects
{
    public class Request: IRequest
    {
        public string Path { get; set; }

        internal Request(string path) => Path = path;
    }
}
