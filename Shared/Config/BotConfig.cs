using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Shared.Config;

public class BotConfig: CommonConfig
{
    public string ApiUri { get; set; } = "http://api.telegram.org";
    public string ApiKey { get; set; } = string.Empty;
    public List<string> Usernames { get; set; } = new();

    [JsonIgnore]
    private const char UsernamesSeparator = ';';

    [JsonIgnore]
    public string UsernamesString
    {
        get => string.Join(UsernamesSeparator, Usernames);
        set => Usernames = value.Split(UsernamesSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }

    public override bool Equals(object? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is not BotConfig that) return false;

        if (Name != that.Name || Autostart != that.Autostart || ApiUri != that.ApiUri ||
            ApiKey != that.ApiKey || Usernames.Count != that.Usernames.Count) return false;

        return !Usernames.Where((x, i) => x != that.Usernames[i]).Any();
    }
}