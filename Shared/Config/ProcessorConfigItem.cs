using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shared.Config;

public class ProcessorConfigItem
{
    public string Name { get; set; } = "";
    public bool Autostart { get; set; } = true;

    #region Server
    public string? Scheme { get; set; }
    public string? Host { get; set; }
    public int? Port { get; set; }

    [JsonIgnore]
    public Uri? Uri
    {
        get
        {
            try
            {
                return new UriBuilder(Scheme, Host, Port ?? 0).Uri;
            }
            catch
            {
                return null;
            }
        }
        set
        {
            Scheme = value?.Scheme;
            Host = value?.Host;
            Port = value?.Port;
        }
    }
    #endregion

    #region Bot
    public string? ApiUri { get; set; }
    public string? ApiKey { get; set; }
    public ICollection<string> Usernames { get; set; } = new List<string>();
    #endregion
}