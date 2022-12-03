using System.Text.Json.Serialization;

namespace Bots.Telegram.ApiObjects.Response;

internal class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("is_bot")]
    public bool IsBot { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("language_code")]
    public string? LanguageCode { get; set; }

    [JsonPropertyName("is_premium")]
    public bool? IsPremium { get; set; }

    [JsonPropertyName("added_to_attachment_menu")]
    public bool? AddedToAttachmentMenu { get; set; }

    [JsonPropertyName("can_join_groups")]
    public bool? CanJoinGroups { get; set; }

    [JsonPropertyName("can_read_all_group_messages")]
    public bool? CanReadAllGroupMessages { get; set; }

    [JsonPropertyName("supports_inline_queries")]
    public bool? SupportInlineQueries { get; set; }
}