using System.Text.Json.Serialization;

namespace Servers.Listeners.Telegram.ApiObjects.Response.Keyboard;

public class InlineKeyboardMarkup : KeyboardMarkup
{
    [JsonPropertyName("inline_keyboard")] 
    public InlineKeyboardButton[] InlineKeyboard { get; set; } = [];
}