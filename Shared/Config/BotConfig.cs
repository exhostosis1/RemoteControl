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
}