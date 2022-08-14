using System.Net;
using RemoteControl.App.Interfaces.Web;

namespace RemoteControl.App.Web.DataObjects
{
    internal class Response : IResponse
    {
        public string ContentType { get; set; } = "text/plain";
        public byte[] Payload { get; set; } = Array.Empty<byte>();
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    }
}
