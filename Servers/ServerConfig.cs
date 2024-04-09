using System.Text.Json.Serialization;

namespace Servers;

public class ServerConfig(ServerType type)
{
    public ServerType Type => type;
    public string Name { get; set; } = string.Empty;
    public bool AutoStart { get; set; }

    #region Web
    public string Scheme { get; set; } = "http";
    public string Host { get; set;  } = "localhost";
    public int Port { get; set;  } = 80;

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
    #endregion

    #region Bot
    public string ApiUri { get; set; } = "http://api.telegram.org";
    public string ApiKey { get; set; } = string.Empty;
    public List<string> Usernames { get; set; } = [];

    [JsonIgnore]
    private const char UsernamesSeparator = ';';

    [JsonIgnore]
    public string UsernamesString
    {
        get => string.Join(UsernamesSeparator, Usernames);
        set => Usernames =
        [
            .. value.Split(UsernamesSeparator,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
        ];
    }
    #endregion
}