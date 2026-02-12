namespace BaseFaq.Models.Faq.Dtos.Faq;

public class FaqGetRequestDto
{
    public bool IncludeFaqItems { get; set; }
    public bool IncludeContentRefs { get; set; }
    public bool IncludeTags { get; set; }
}