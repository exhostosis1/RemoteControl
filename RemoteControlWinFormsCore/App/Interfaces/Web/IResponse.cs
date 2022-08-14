using System.Net;

namespace RemoteControl.App.Interfaces.Web
{
    public interface IResponse
    {
        public string ContentType { get; set; }
        public byte[] Payload { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
