using System.Text.Json.Serialization;

namespace Bots.Telegram.ApiObjects.Response;

internal class ReplyKeyboardMarkup
{
    [JsonPropertyName("keyboard")] 
    public KeyboardButton[][] Keyboard { get; set; } = Array.Empty<KeyboardButton[]>();

    [JsonPropertyName("resize_keyboard")]
    public bool? ResizeKeyboard { get; set; }

    [JsonPropertyName("one_time_keyboard")]
    public bool? OneTimeKeyboard { get; set; }

    [JsonPropertyName("input_field_placeholder")]
    public string? InputFieldPlaceholder { get; set; }

    [JsonPropertyName("selective")]
    public bool? Selective { get; set; }
}