using RemoteControlCore.Interfaces;

namespace RemoteControlCore.Abstract
{
    internal abstract class AbstractController
    {
        public virtual void ProcessRequest(IHttpRequestArgs context)
        {
        }

        public virtual void ProcessSimpleRequest(IHttpRequestArgs context)
        {
        }

        public virtual string ProcessApiRequest(string message)
        {
            return null;
        }
    }
}
