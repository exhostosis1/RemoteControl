using System.Text.Json.Serialization;

namespace MainApp.Workers.Listeners.Telegram.ApiObjects.Response.Keyboard;

internal class InlineKeyboardMarkup : KeyboardMarkup
{
    [JsonPropertyName("inline_keyboard")]
    public InlineKeyboardButton[] InlineKeyboard { get; set; } = [];
}