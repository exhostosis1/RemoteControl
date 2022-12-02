using System;
using System.ComponentModel;

namespace Shared.Config;

[DisplayName("Server")]
public class ServerConfig
{
    [DisplayName("Scheme")]
    public string Scheme { get; set; } = "http";

    [DisplayName("Host")]
    public string Host { get; set; } = "localhost";

    [DisplayName("Port")]
    public int Port { get; set; } = 80;

    public Uri Uri => new UriBuilder(Scheme, Host, Port).Uri;

    [DisplayName("Autostart")]
    public bool StartListening { get; set; } = true;
}