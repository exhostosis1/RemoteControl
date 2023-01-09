using System;
using System.Text.Json.Serialization;

namespace Shared.Bot;

public interface IMessage
{
    [JsonPropertyName("from")]
    public IUser? From { get; set; }
    public IChat? Chat { get; set; }
    public int Date { get; set; }
    public DateTime ParsedDate => DateTimeOffset.FromUnixTimeSeconds(Date).LocalDateTime;
    public string? Text { get; set; }
}