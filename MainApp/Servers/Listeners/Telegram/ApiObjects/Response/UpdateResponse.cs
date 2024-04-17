using System.Text.Json.Serialization;

namespace MainApp.Servers.Listeners.Telegram.ApiObjects.Response;

internal class UpdateResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("result")]
    public Update[] Result { get; set; } = [];
}