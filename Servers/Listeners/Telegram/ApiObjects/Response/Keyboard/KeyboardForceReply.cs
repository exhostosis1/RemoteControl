using System.Text.Json.Serialization;

namespace Servers.Listeners.Telegram.ApiObjects.Response.Keyboard;

public class KeyboardForceReply : KeyboardMarkup
{
    [JsonPropertyName("force_reply")]
    public bool ForceReply { get; set; } = true;

    [JsonPropertyName("input_field_placeholder")]
    public string? InputFieldPlaceholder { get; set; }

    [JsonPropertyName("selective")]
    public bool? Selective { get; set; }
}