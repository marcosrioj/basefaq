namespace Querify.Models.Broadcast.Enums;

/// <summary>
/// Classifies the interaction shape represented by a captured Broadcast item.
/// </summary>
public enum ItemKind
{
    /// <summary>
    /// The item is a top-level publication that can anchor a public interaction thread.
    /// </summary>
    Post = 1,

    /// <summary>
    /// The item is a public reply or nested contribution within a Broadcast thread.
    /// </summary>
    Comment = 6,

    /// <summary>
    /// The item is a message captured from a group, channel, community, or other shared messaging surface.
    /// </summary>
    SharedMessage = 11,

    /// <summary>
    /// The item has a supported interaction shape that is not yet represented by a dedicated classification.
    /// </summary>
    Other = 16
}
