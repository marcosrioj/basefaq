namespace BaseFaq.Models.Faq.Dtos.FaqContentRef;

public class FaqContentRefCreateRequestDto
{
    public required Guid FaqId { get; set; }
    public required Guid ContentRefId { get; set; }
}