using System.Text.Json.Serialization;

namespace Shared.Bots.Telegram.ApiObjects.Response.Keyboard;

public class InlineKeyboardMarkup : KeyboardMarkup
{
    [JsonPropertyName("inline_keyboard")]
    public InlineKeyboardButton[] InlineKeyboard { get; set; }
}