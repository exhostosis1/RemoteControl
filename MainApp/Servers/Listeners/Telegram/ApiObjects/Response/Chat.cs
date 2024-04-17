using System.Text.Json.Serialization;

namespace MainApp.Servers.Listeners.Telegram.ApiObjects.Response;

internal class Chat
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "private";

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("is_forum")]
    public bool? IsForum { get; set; }

    [JsonPropertyName("photo")]
    public ChatPhoto? Photo { get; set; }

    [JsonPropertyName("active_usernames")]
    public string[]? ActiveUsernames { get; set; }

    [JsonPropertyName("emoji_status_custom_emoji_id")]
    public string? EmojiStatusCustomEmojiId { get; set; }

    [JsonPropertyName("bio")]
    public string? Bio { get; set; }

    [JsonPropertyName("has_private_forwards")]
    public bool? HasPrivateForwards { get; set; }

    [JsonPropertyName("has_restricted_voice_and_video_messages")]
    public bool? HasRestrictedVoiceAndVideoMessages { get; set; }

    [JsonPropertyName("join_to_send_messages")]
    public bool? JoinToSendMessages { get; set; }

    [JsonPropertyName("join_by_request")]
    public bool? JoinByRequest { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("invite_link")]
    public string? InviteLink { get; set; }

    [JsonPropertyName("pinned_message")]
    public Message? PinnedMessage { get; set; }

    [JsonPropertyName("permissions")]
    public ChatPermissions? ChatPermissions { get; set; }

    [JsonPropertyName("slow_mode_delay")]
    public int? SlowModeDelay { get; set; }

    [JsonPropertyName("message_auto_delete_time")]
    public int? MessageAutoDeleteTime { get; set; }

    [JsonPropertyName("has_protected_content")]
    public bool? HasProtectedContent { get; set; }

    [JsonPropertyName("sticker_set_name")]
    public string? StickerSetName { get; set; }

    [JsonPropertyName("can_set_sticker_set")]
    public bool? CanSetStickerSet { get; set; }

    [JsonPropertyName("linked_chat_id")]
    public int? LinkedChatId { get; set; }

    [JsonPropertyName("location")]
    public ChatLocation? ChatLocation { get; set; }
}