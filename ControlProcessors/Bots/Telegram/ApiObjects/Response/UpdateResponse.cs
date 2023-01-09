using System.Text.Json.Serialization;
using Shared.Bot;

namespace Bots.Telegram.ApiObjects.Response;

internal class UpdateResponse: IUpdateResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("result")]
    public IUpdate[] Result { get; set; } = Array.Empty<IUpdate>();
}