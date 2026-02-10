namespace BaseFaq.Models.Faq.Dtos.Tag;

public class TagDto
{
    public required Guid Id { get; set; }
    public required string Value { get; set; }
    public required Guid TenantId { get; set; }
}