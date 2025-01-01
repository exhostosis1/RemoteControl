using MainApp.Servers.Listeners.Telegram.ApiObjects.Response;
using MainApp.Servers.Listeners.Telegram.ApiObjects.Response.Keyboard;
using System.Text.Json.Serialization;

namespace MainApp.Servers.Listeners.Telegram.ApiObjects.Request;

internal class SendMessageParameters
{
    [JsonPropertyName("chat_id")]
    public int ChatId { get; set; }

    [JsonPropertyName("message_thread_id")]
    public int? MessageThreadId { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("parse_mode")]
    public string? ParseMode { get; set; }

    [JsonPropertyName("entities")]
    public MessageEntity[]? Entities { get; set; }

    [JsonPropertyName("disable_web_page_preview")]
    public bool? DisableWebPagePreview { get; set; }

    [JsonPropertyName("disable_notification")]
    public bool? DisableNotification { get; set; }

    [JsonPropertyName("protect_content")]
    public bool? ProtectContent { get; set; }

    [JsonPropertyName("reply_to_message_id")]
    public int? ReplyToMessageId { get; set; }

    [JsonPropertyName("allow_sending_without_reply")]
    public bool? AllowSendingWithoutReply { get; set; }

    [JsonPropertyName("reply_markup")]
    public KeyboardMarkup? ReplyMarkup { get; set; }
}