namespace BaseFaq.Models.Faq.Dtos.FaqTag;

public class FaqTagCreateRequestDto
{
    public required Guid FaqId { get; set; }
    public required Guid TagId { get; set; }
}