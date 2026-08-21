namespace Querify.Models.Tenant.Enums;

/// <summary>
/// Identifies the provider surface managed by a tenant channel connection.
/// </summary>
public enum ChannelConnectionKind
{
    /// <summary>
    /// Connects an embedded website chat surface used for private visitor conversations.
    /// </summary>
    WebChat = 1,

    /// <summary>
    /// Connects Instagram messaging, comments, mentions, or other supported Instagram interactions.
    /// </summary>
    Instagram = 6,

    /// <summary>
    /// Connects Facebook Messenger conversations associated with the tenant's provider account.
    /// </summary>
    Messenger = 11,

    /// <summary>
    /// Connects TikTok messaging, comments, or other supported TikTok interactions.
    /// </summary>
    TikTok = 16,

    /// <summary>
    /// Connects Snapchat conversations or shared interactions supported by the provider integration.
    /// </summary>
    Snapchat = 21,

    /// <summary>
    /// Connects WhatsApp conversations or shared channel interactions for the configured business account.
    /// </summary>
    WhatsApp = 26,

    /// <summary>
    /// Connects Telegram chats, groups, or channels handled by the configured bot or account.
    /// </summary>
    Telegram = 31,

    /// <summary>
    /// Connects an email inbox used to receive and send asynchronous interactions.
    /// </summary>
    Email = 36,

    /// <summary>
    /// Connects an SMS-capable phone number used for text-message interactions.
    /// </summary>
    Sms = 41,

    /// <summary>
    /// Connects a supported provider that does not yet have a dedicated channel classification.
    /// </summary>
    Other = 46
}
