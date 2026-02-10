namespace BaseFaq.Models.Faq.Dtos.FaqContentRef;

public class FaqContentRefDto
{
    public required Guid Id { get; set; }
    public required Guid FaqId { get; set; }
    public required Guid ContentRefId { get; set; }
    public required Guid TenantId { get; set; }
}