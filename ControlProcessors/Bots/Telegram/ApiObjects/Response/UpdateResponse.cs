using System.Text.Json.Serialization;

namespace Bots.Telegram.ApiObjects.Response;

internal class UpdateResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("result")]
    public Update[] Result { get; set; } = Array.Empty<Update>();
}