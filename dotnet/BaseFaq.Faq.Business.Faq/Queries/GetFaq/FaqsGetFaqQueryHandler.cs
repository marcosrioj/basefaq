using BaseFaq.Faq.Persistence.FaqDb.Repositories;
using BaseFaq.Models.Faq.Dtos.Faq;
using MediatR;

namespace BaseFaq.Faq.Business.Faq.Queries.GetFaq;

public class FaqsGetFaqQueryHandler(IFaqRepository faqRepository) : IRequestHandler<FaqsGetFaqQuery, FaqDto?>
{
    public async Task<FaqDto?> Handle(FaqsGetFaqQuery request, CancellationToken cancellationToken)
    {
        var faq = await faqRepository.GetByIdAsync(request.Id, cancellationToken);
        if (faq is null)
        {
            return null;
        }

        return new FaqDto
        {
            Id = faq.Id,
            Name = faq.Name,
            Language = faq.Language,
            Status = faq.Status,
            SortType = faq.SortType,
            TenantId = faq.TenantId
        };
    }
}