using System.Net;
using Servers.DataObjects.BotButtons;

namespace Servers.DataObjects;

public class OutputContext
{
    #region Web
    public string ContentType { get; set; } = "text/plain";
    public byte[] Payload { get; set; } = [];
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    #endregion

    #region Bot
    public string Message { get; set; } = string.Empty;
    public IButtonsMarkup? Buttons { get; set; }
    #endregion
}