using BaseFaq.Models.Faq.Enums;

namespace BaseFaq.Models.Faq.Dtos.Faq;

public class FaqUpdateRequestDto
{
    public required string Name { get; set; }
    public required string Language { get; set; }
    public required FaqStatus Status { get; set; }
    public required FaqSortType SortType { get; set; }
}