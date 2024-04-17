using MainApp.Interfaces;

namespace MainApp.DTO;

public class WebServerDto: IWebServer
{
    public string Name { get; set; } = "";
    public Guid Id { get; set; }
    public bool Status { get; set; }
    public bool IsAutostart { get; set; }
    public Uri? ListeningUri { get; set; }
}