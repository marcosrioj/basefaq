using BaseFaq.Models.Common.Dtos;

namespace BaseFaq.Models.Faq.Dtos.FaqItem;

public class FaqItemSearchRequestDto : PagedResultRequestDto
{
    public string? Search { get; set; }
    public List<Guid>? FaqIds { get; set; }
}