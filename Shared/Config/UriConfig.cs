using System;
using System.ComponentModel;
using Shared.Config.Interfaces;

namespace Shared.Config;

[DisplayName("Uri")]
public class UriConfig : IConfigItem
{
    [DisplayName("host")]
    public string Host { get; set; } = "localhost";

    [DisplayName("scheme")]
    public string Scheme { get; set; } = "http";

    [DisplayName("port")]
    public int Port { get; set; } = 1488;

    public Uri Uri => new UriBuilder(Scheme, Host, Port).Uri;

    public override string ToString() => $"{Scheme}://{Host}:{Port}/";
}