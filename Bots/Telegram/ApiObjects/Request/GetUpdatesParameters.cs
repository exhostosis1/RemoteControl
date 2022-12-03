using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bots.Telegram.ApiObjects.Request;

internal class GetUpdatesParameters
{
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    [JsonPropertyName("limit")]
    [Range(0, 100)]
    public int? Limit { get; set; }

    [JsonPropertyName("timeout")]
    public int? Timeout { get; set; }

    [JsonPropertyName("allowed_updates")]
    public string[]? AllowedUpdates { get; set; }
}