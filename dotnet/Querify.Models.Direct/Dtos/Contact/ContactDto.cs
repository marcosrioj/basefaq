namespace Querify.Models.Direct.Dtos.Contact;

public sealed class ContactDto
{
    public required Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required string GivenName { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? PhotoUrl { get; set; }
    public string? TimeZone { get; set; }
    public string? PhoneNumber { get; set; }
    public string? InstagramProfileUrl { get; set; }
    public string? TikTokProfileUrl { get; set; }
    public string? FacebookProfileUrl { get; set; }
    public string? SnapchatProfileUrl { get; set; }
    public required int ConversationCount { get; set; }
    public DateTime? CreatedAtUtc { get; set; }
    public DateTime? LastUpdatedAtUtc { get; set; }
}
