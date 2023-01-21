using System.Text.Json.Serialization;

namespace Shared.Bots.Telegram.ApiObjects.Response.Keyboard;

public class ReplyKeyboardRemove : KeyboardMarkup
{
    [JsonPropertyName("remove_keyboard")]
    public bool RemoveKeyboard { get; set; } = true;

    [JsonPropertyName("selective")]
    public bool? Selective { get; set; }
}