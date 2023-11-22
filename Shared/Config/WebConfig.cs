using System;
using System.Text.Json.Serialization;

namespace Shared.Config;

public class WebConfig : CommonConfig
{
    public string Scheme { get; init; } = "http";
    public string Host { get; init;  } = "localhost";
    public int Port { get; init;  } = 80;

    [JsonIgnore]
    public Uri Uri
    {
        get => new UriBuilder(Scheme, Host, Port).Uri;
        init
        {
            Scheme = value.Scheme;
            Host = value.Host;
            Port = value.Port;
        }
    }

    public override bool Equals(object? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is not WebConfig that) return false;

        return Name == that.Name && AutoStart == that.AutoStart && Scheme == that.Scheme &&
               Host == that.Host && Port == that.Port;
    }

    protected bool Equals(WebConfig other)
    {
        return Scheme == other.Scheme && Host == other.Host && Port == other.Port;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Scheme, Host, Port);
    }
}