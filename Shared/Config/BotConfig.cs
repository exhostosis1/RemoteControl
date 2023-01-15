using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Shared.Config;

public class BotConfig: CommonConfig
{
    public string ApiUri { get; set; } = string.Empty;
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

    public override bool Equals(object? obj)
    {
        if(this == obj) return true;
        if(obj is not BotConfig that) return false;

        if (this.Name != that.Name || this.Autostart != that.Autostart || this.ApiUri != that.ApiUri ||
            this.ApiKey != that.ApiKey || this.Usernames.Count != that.Usernames.Count) return false;

        return !this.Usernames.Where((x, i) => x != that.Usernames[i]).Any();
    }
}