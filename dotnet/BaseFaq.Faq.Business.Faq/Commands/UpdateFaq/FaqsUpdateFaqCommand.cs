using BaseFaq.Models.Faq.Enums;
using MediatR;

namespace BaseFaq.Faq.Business.Faq.Commands.UpdateFaq;

public class FaqsUpdateFaqCommand : IRequest
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Language { get; set; }
    public required FaqStatus Status { get; set; }
    public required FaqSortType SortType { get; set; }
}