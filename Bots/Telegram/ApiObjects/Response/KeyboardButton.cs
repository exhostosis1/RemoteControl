using System.Text.Json.Serialization;

namespace Bots.Telegram.ApiObjects.Response;

internal class KeyboardButton
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("request_contact")]
    public bool? RequestContact { get; set; }

    [JsonPropertyName("request_location")]
    public bool? RequestLocation { get; set; }

    [JsonPropertyName("request_poll")]
    public KeyboardButtonPollType? RequestPoll { get; set; }

    [JsonPropertyName("web_app")]
    public WebAppInfo? WebApp { get; set; }
}