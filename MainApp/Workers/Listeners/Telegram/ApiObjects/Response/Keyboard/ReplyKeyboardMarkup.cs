using System.Text.Json.Serialization;

namespace MainApp.Workers.Listeners.Telegram.ApiObjects.Response.Keyboard;

internal class ReplyKeyboardMarkup : KeyboardMarkup
{
    [JsonPropertyName("keyboard")]
    public KeyboardButton[][] Keyboard { get; set; } = [];

    [JsonPropertyName("is_persistent")]
    public bool? Persistent { get; set; }

    [JsonPropertyName("resize_keyboard")]
    public bool? ResizeKeyboard { get; set; }

    [JsonPropertyName("one_time_keyboard")]
    public bool? OneTimeKeyboard { get; set; }

    [JsonPropertyName("input_field_placeholder")]
    public string? InputFieldPlaceholder { get; set; }

    [JsonPropertyName("selective")]
    public bool? Selective { get; set; }
}