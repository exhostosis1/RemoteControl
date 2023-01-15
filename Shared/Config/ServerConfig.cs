using System;
using System.Text.Json.Serialization;

namespace Shared.Config;

public class ServerConfig: CommonConfig
{
    public string Scheme { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }

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

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if(obj is not ServerConfig that) return false;

        return this.Name == that.Name && this.Autostart == that.Autostart && this.Scheme == that.Scheme &&
               this.Host == that.Host && this.Port == that.Port;
    }
}