using RemoteControl.App.Web.Interfaces;

namespace RemoteControl.App.Web.Controllers
{
    internal abstract class AbstractController
    {
        public abstract void ProcessRequest(IHttpRequestArgs context);
    }
}
