using System;

namespace Shared.Config;

public class ServerConfig
{
    public string Scheme { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }

    public Uri Uri => new UriBuilder(Scheme, Host, Port).Uri;

    public bool StartListening { get; set; } = true;
}