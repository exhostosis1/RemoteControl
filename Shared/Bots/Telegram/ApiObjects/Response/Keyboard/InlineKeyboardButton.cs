using System.Text.Json.Serialization;

namespace Shared.Bots.Telegram.ApiObjects.Response.Keyboard;

public class InlineKeyboardButton
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("callback_data")]
    public string? CallbackData { get; set; }

    [JsonPropertyName("web_app")]
    public WebAppInfo? WebApp { get; set; }

    [JsonPropertyName("login_url")]
    public LoginUrl? LoginUrl { get; set; }

    [JsonPropertyName("switch_inline_query")]
    public string? SwitchInlineQuery { get; set; }

    [JsonPropertyName("switch_inline_query_current_chat")]
    public string? SwitchInlineQueryCurrentChat { get; set; }

    [JsonPropertyName("callback_game")]
    public CallbackGame? CallbackGame { get; set; }

    [JsonPropertyName("pay")]
    public bool? Pay { get; set; }

}