namespace BaseFaq.Models.Faq.Dtos.FaqTag;

public class FaqTagDto
{
    public required Guid Id { get; set; }
    public required Guid FaqId { get; set; }
    public required Guid TagId { get; set; }
    public required Guid TenantId { get; set; }
}