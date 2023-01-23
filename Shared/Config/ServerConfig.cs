using System;
using System.Text.Json.Serialization;

namespace Shared.Config;

public class ServerConfig : CommonConfig
{
    public string Scheme { get; set; } = "http";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 80;

    [JsonIgnore]
    public Uri Uri
    {
        get => new UriBuilder(Scheme, Host, Port).Uri;
        set
        {
            Scheme = value.Scheme;
            Host = value.Host;
            Port = value.Port;
        }
    }

    public override bool Equals(object? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is not ServerConfig that) return false;

        return Name == that.Name && Autostart == that.Autostart && Scheme == that.Scheme &&
               Host == that.Host && Port == that.Port;
    }
}