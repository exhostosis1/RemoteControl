using System.Text.Json.Serialization;

namespace MainApp.Workers.Listeners.Telegram.ApiObjects.Response.Keyboard;

internal class ReplyKeyboardRemove : KeyboardMarkup
{
    [JsonPropertyName("remove_keyboard")]
    public bool RemoveKeyboard { get; set; } = true;

    [JsonPropertyName("selective")]
    public bool? Selective { get; set; }
}