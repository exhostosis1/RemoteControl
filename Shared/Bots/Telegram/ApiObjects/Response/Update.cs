using System.Text.Json.Serialization;

namespace Shared.Bots.Telegram.ApiObjects.Response;

public class Update
{
    [JsonPropertyName("update_id")]
    public int UpdateId { get; set; }

    [JsonPropertyName("message")]
    public Message? Message { get; set; }

    [JsonPropertyName("edited_message")]
    public Message? EditedMessage { get; set; }

    [JsonPropertyName("channel_post")]
    public Message? ChannelPost { get; set; }

    [JsonPropertyName("edited_channel_post")]
    public Message? EditedChannelPostMessage { get; set; }

    [JsonPropertyName("inline_query")]
    public InlineQuery? InlineQuery { get; set; }

    [JsonPropertyName("chosen_inline_result")]
    public ChosenInlineResult? ChosenInlineResult { get; set; }

    [JsonPropertyName("callback_query")]
    public CallbackQuery? CallbackQuery { get; set; }

    [JsonPropertyName("shipping_query")]
    public ShippingQuery? ShippingQuery { get; set; }

    [JsonPropertyName("pre_checkout_query")]
    public PreCheckoutQuery? PreCheckoutQuery { get; set; }

    [JsonPropertyName("poll")]
    public Poll? Poll { get; set; }

    [JsonPropertyName("poll_answer")]
    public PollAnswer? PollAnswer { get; set; }

    [JsonPropertyName("my_chat_member")]
    public ChatMemberUpdated? MyChatMember { get; set; }

    [JsonPropertyName("chat_member")]
    public ChatMemberUpdated? ChatMember { get; set; }

    [JsonPropertyName("chat_join_request")]
    public ChatJoinRequest? ChatJoinRequest { get; set; }
}