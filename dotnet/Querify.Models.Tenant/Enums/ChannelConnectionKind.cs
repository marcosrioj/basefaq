namespace Querify.Models.Direct.Enums;

/// <summary>
/// Describes where a support conversation started.
/// </summary>
public enum ChannelConnectionKind
{
    /// <summary>
    /// Conversation started from an embedded web chat surface.
    /// </summary>
    WebChat = 1,

    Instagram = 6,

    Messenger = 11,

    Tiktok = 16,

    Snapchat = 21,

    WhatsApp = 26,

    Telegram = 31,

    /// <summary>
    /// Conversation originated from email and may represent an asynchronous support thread.
    /// </summary>
    Email = 36,

    /// <summary>
    /// Conversation originated from email and may represent an asynchronous support thread.
    /// </summary>
    Sms = 41,

    /// <summary>
    /// Conversation source is known by Direct but not represented by a more specific channel yet.
    /// </summary>
    Other = 99
}