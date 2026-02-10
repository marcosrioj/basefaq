using BaseFaq.Models.Faq.Enums;

namespace BaseFaq.Models.Faq.Dtos.ContentRef;

public class ContentRefCreateRequestDto
{
    public required ContentRefKind Kind { get; set; }
    public required string Locator { get; set; }
    public string? Label { get; set; }
    public string? Scope { get; set; }
}