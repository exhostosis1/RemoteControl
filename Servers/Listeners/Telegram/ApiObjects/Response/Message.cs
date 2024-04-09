using System.Text.Json.Serialization;

namespace Servers.Listeners.Telegram.ApiObjects.Response;

public class Message
{
    [JsonPropertyName("message_id")]
    public int MessageId { get; set; }

    [JsonPropertyName("message_thread_id")]
    public int? MessageThreadId { get; set; }

    [JsonPropertyName("from")]
    public User? From { get; set; }

    [JsonPropertyName("sender_chat")]
    public User? SenderChat { get; set; }

    [JsonPropertyName("chat")]
    public Chat? Chat { get; set; }

    [JsonPropertyName("date")]
    public int Date { get; set; }

    [JsonIgnore]
    public DateTime ParsedDate
    {
        get => DateTimeOffset.FromUnixTimeSeconds(Date).LocalDateTime;
        set => Date = (int)((DateTimeOffset)value).ToUnixTimeSeconds();
    }

    [JsonPropertyName("forward_from")]
    public User? ForwardFrom { get; set; }

    [JsonPropertyName("forward_from_chat")]
    public Chat? ForwardFromChat { get; set; }

    [JsonPropertyName("forward_from_message_id")]
    public int? ForwardFromMessageId { get; set; }

    [JsonPropertyName("forward_signature")]
    public string? ForwardSignature { get; set; }

    [JsonPropertyName("forward_sender_name")]
    public string? ForwardSenderName { get; set; }

    [JsonPropertyName("forward_date")]
    public int? ForwardDate { get; set; }

    [JsonPropertyName("is_topic_message")]
    public bool? IsTopicMessage { get; set; }

    [JsonPropertyName("is_automatic_forward")]
    public bool? IsAutomaticForward { get; set; }

    [JsonPropertyName("reply_to_message")]
    public Message? ReplyToMessage { get; set; }

    [JsonPropertyName("via_bot")]
    public User? ViaBot { get; set; }

    [JsonPropertyName("edit_date")]
    public int? EditDate { get; set; }

    [JsonPropertyName("has_protected_content")]
    public bool? HasProtectedContent { get; set; }

    [JsonPropertyName("media_group_id")]
    public string? MediaGroupId { get; set; }

    [JsonPropertyName("author_signature")]
    public string? AuthorSignature { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("entities")]
    public MessageEntity[]? Entities { get; set; }

    [JsonPropertyName("animation")]
    public Animation? Animation { get; set; }

    [JsonPropertyName("audio")]
    public Audio? Audio { get; set; }

    [JsonPropertyName("document")]
    public Document? Document { get; set; }

    [JsonPropertyName("photo")]
    public PhotoSize[]? Photo { get; set; }

    [JsonPropertyName("sticker")]
    public Sticker? Sticker { get; set; }

    [JsonPropertyName("video")]
    public Video? Video { get; set; }

    [JsonPropertyName("video_note")]
    public VideoNote? VideoNote { get; set; }

    [JsonPropertyName("voice")]
    public Voice? Voice { get; set; }

    [JsonPropertyName("caption")]
    public string? Caption { get; set; }

    [JsonPropertyName("caption_entities")]
    public MessageEntity[]? CaptionEntities { get; set; }

    [JsonPropertyName("contact")]
    public Contact? Contact { get; set; }

    [JsonPropertyName("dice")]
    public Dice? Dice { get; set; }

    [JsonPropertyName("game")]
    public Game? Game { get; set; }

    [JsonPropertyName("venue")]
    public Venue? Venue { get; set; }

    [JsonPropertyName("location")]
    public Location? Location { get; set; }

    [JsonPropertyName("new_chat_members")]
    public User[]? NewChatMembers { get; set; }

    [JsonPropertyName("left_chat_member")]
    public User? LeftChatMember { get; set; }

    [JsonPropertyName("new_chat_title")]
    public string? NewChatTitle { get; set; }

    [JsonPropertyName("new_chat_photo")]
    public PhotoSize[]? NewChatPhoto { get; set; }

    [JsonPropertyName("delete_chat_photo")]
    public bool? DeleteChatPhoto { get; set; }

    [JsonPropertyName("group_chat_created")]
    public bool? GroupChatCreated { get; set; }

    [JsonPropertyName("supergroup_chat_created")]
    public bool? SupergroupChatCreated { get; set; }

    [JsonPropertyName("channel_chat_created")]
    public bool? ChannelChatCreated { get; set; }

    [JsonPropertyName("message_auto_delete_timer_changed")]
    public MessageAutoDeleteTimerChanged? MessageAutoDeleteTimer { get; set; }

    [JsonPropertyName("migrate_to_chat_id")]
    public int? MigrateToChatId { get; set; }

    [JsonPropertyName("migrate_from_chat_id")]
    public int? MigrateFromChatId { get; set; }

    [JsonPropertyName("pinned_message")]
    public Message? PinnedMessage { get; set; }

    [JsonPropertyName("invoice")]
    public Invoice? Invoice { get; set; }

    [JsonPropertyName("successful_payment")]
    public SuccessfulPayment? SuccessfulPayment { get; set; }

    [JsonPropertyName("connected_website")]
    public string? ConnectedWebsite { get; set; }

    [JsonPropertyName("passport_data")]
    public PassportData? PassportData { get; set; }

    [JsonPropertyName("proximity_alert_triggered")]
    public ProximityAlertTriggered? ProximityAlertTriggered { get; set; }

    [JsonPropertyName("forum_topic_created")]
    public ForumTopicCreated? ForumTopicCreated { get; set; }

    [JsonPropertyName("forum_topic_closed")]
    public ForumTopicClosed? ForumTopicClosed { get; set; }

    [JsonPropertyName("forum_topic_reopened")]
    public ForumTopicReopened? ForumTopicReopened { get; set; }

    [JsonPropertyName("video_chat_scheduled")]
    public VideoChatScheduled? VideoChatScheduled { get; set; }

    [JsonPropertyName("video_chat_started")]
    public VideoChatScheduled? VideoChatStarted { get; set; }

    [JsonPropertyName("video_chat_ended")]
    public VideoChatScheduled? VideoChatEnded { get; set; }

    [JsonPropertyName("video_chat_participants_invited")]
    public VideoChatParticipantsInvited? VideoChatParticipantsInvited { get; set; }

    [JsonPropertyName("web_app_data")]
    public WebAppData? WebAppData { get; set; }

    [JsonPropertyName("reply_markup")]
    public InlineKeyboardMarkup? ReplyMarkup { get; set; }
}