using System;
using System.Text.Json.Serialization;

namespace Shared.Config;

public class ServerConfig: CommonConfig, IEquatable<ServerConfig>
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

    public bool Equals(ServerConfig? that)
    {
        if (ReferenceEquals(null, that)) return false;
        if (ReferenceEquals(this, that)) return true;

        return Name == that.Name && Autostart == that.Autostart && Scheme == that.Scheme &&
               Host == that.Host && Port == that.Port;
    }
}