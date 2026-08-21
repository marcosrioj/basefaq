namespace Querify.Models.Tenant.Enums;

/// <summary>
/// Describes the operational lifecycle of a tenant channel connection.
/// </summary>
public enum ChannelConnectionStatus
{
    /// <summary>
    /// The connection was configured but has not completed its first successful provider authorization.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Provider authorization is valid and the connection is available for enabled module workflows.
    /// </summary>
    Connected = 6,

    /// <summary>
    /// The provider link was intentionally disconnected and requires an explicit reconnect operation.
    /// </summary>
    Disconnected = 11,

    /// <summary>
    /// Provider credentials expired and must be renewed before the connection can be used.
    /// </summary>
    Expired = 16,

    /// <summary>
    /// The latest connection or synchronization attempt failed and requires operator attention.
    /// </summary>
    Error = 21,

    /// <summary>
    /// Querify or the provider suspended the connection, so workflows must not use it until reinstated.
    /// </summary>
    Suspended = 26
}
