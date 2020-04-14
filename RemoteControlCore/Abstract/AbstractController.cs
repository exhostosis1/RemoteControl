using RemoteControlCore.Interfaces;

namespace RemoteControlCore.Abstract
{
    internal abstract class AbstractController
    {
        public abstract void ProcessRequest(IHttpRequestArgs context);
    }
}
