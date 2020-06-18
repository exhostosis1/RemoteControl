using System.IO;
using RemoteControlCore.Interfaces;

namespace RemoteControlCore.Abstract
{
    internal abstract class AbstractController
    {
        public virtual void ProcessRequest(IHttpRequestArgs context)
        {
        }

        public virtual void ProcessApiRequest(string message, Stream stream)
        {
        }
    }
}
