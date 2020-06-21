using RemoteControl.Core.Interfaces;

namespace RemoteControl.Core.Abstract
{
    internal abstract class AbstractController
    {
        public abstract void ProcessRequest(IHttpRequestArgs context);
    }
}
