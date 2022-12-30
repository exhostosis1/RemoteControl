using System;
using System.Text.Json.Serialization;

namespace Shared.Config;

public class ServerConfig: CommonConfig
{
    public string Scheme { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }

    [JsonIgnore]
    public Uri? Uri
    {
        get
        {
            try
            {
                return new UriBuilder(Scheme, Host, Port).Uri;
            }
            catch
            {
                return null;
            }
        }
        set
        {
            Scheme = value?.Scheme ?? string.Empty;
            Host = value?.Host ?? string.Empty;
            Port = value?.Port ?? 0;
        }
    }
}