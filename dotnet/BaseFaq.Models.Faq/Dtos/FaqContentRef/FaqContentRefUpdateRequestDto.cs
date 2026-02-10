namespace BaseFaq.Models.Faq.Dtos.FaqContentRef;

public class FaqContentRefUpdateRequestDto
{
    public required Guid FaqId { get; set; }
    public required Guid ContentRefId { get; set; }
}