using System.Net;

namespace Shared.DataObjects.Interfaces
{
    public interface IResponse
    {
        public string ContentType { get; set; }
        public byte[] Payload { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
