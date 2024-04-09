using System.Text.Json.Serialization;

namespace Servers.Listeners.Telegram.ApiObjects.Response;

public class UpdateResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("result")]
    public Update[] Result { get; set; } = [];
}