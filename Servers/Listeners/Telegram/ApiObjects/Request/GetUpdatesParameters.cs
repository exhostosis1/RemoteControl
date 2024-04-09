using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Servers.Listeners.Telegram.ApiObjects.Request;

public class GetUpdatesParameters(int? lastUpdateId)
{
    [JsonPropertyName("offset")]
    public int? Offset { get; set; } = lastUpdateId + 1;

    [JsonPropertyName("limit")]
    [Range(0, 100)]
    public int? Limit { get; set; }

    [JsonPropertyName("timeout")]
    public int? Timeout { get; set; }

    [JsonPropertyName("allowed_updates")]
    public string[]? AllowedUpdates { get; set; }
}