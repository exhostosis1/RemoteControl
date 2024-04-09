using System.Text.Json.Serialization;

namespace Servers.Listeners.Telegram.ApiObjects.Response.Keyboard;

[JsonDerivedType(typeof(ReplyKeyboardMarkup), "reply")]
[JsonDerivedType(typeof(ReplyKeyboardRemove), "remove")]
[JsonDerivedType(typeof(Servers.Listeners.Telegram.ApiObjects.Response.Keyboard.InlineKeyboardMarkup), "inline")]
[JsonDerivedType(typeof(KeyboardForceReply), "force")]
public abstract class KeyboardMarkup
{
}