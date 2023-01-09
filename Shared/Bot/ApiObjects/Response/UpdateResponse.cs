using System;
using System.Text.Json.Serialization;

namespace Shared.Bot.ApiObjects.Response;

public class UpdateResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("result")]
    public Update[] Result { get; set; } = Array.Empty<Update>();
}