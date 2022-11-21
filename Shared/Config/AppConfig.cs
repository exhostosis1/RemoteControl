using System;

namespace Shared.Config;

public record AppConfig
{
    public string Host { get; set; } = "localhost";
    public string Scheme { get; set; } = "http";
    public int Port { get; set; } = 1488;
    public Uri Uri => new UriBuilder(Scheme, Host, Port).Uri;
    public override string ToString() => $"{Scheme}://{Host}:{Port}/";
}