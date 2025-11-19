using System.Text.Json.Serialization;

namespace MainApp.Workers.Listeners.Telegram.ApiObjects.Response.Keyboard;

[JsonDerivedType(typeof(ReplyKeyboardMarkup), "reply")]
[JsonDerivedType(typeof(ReplyKeyboardRemove), "remove")]
[JsonDerivedType(typeof(global::MainApp.Workers.Listeners.Telegram.ApiObjects.Response.Keyboard.InlineKeyboardMarkup), "inline")]
[JsonDerivedType(typeof(KeyboardForceReply), "force")]
internal abstract class KeyboardMarkup
{
}