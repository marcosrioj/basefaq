using BaseFaq.Faq.Persistence.FaqDb.Repositories;
using MediatR;

namespace BaseFaq.Faq.Business.Faq.Commands.CreateFaq;

public class FaqsCreateFaqCommandHandler(IFaqRepository faqRepository) : IRequestHandler<FaqsCreateFaqCommand, Guid>
{
    public async Task<Guid> Handle(FaqsCreateFaqCommand request, CancellationToken cancellationToken)
    {
        var faq = new Persistence.FaqDb.Entities.Faq
        {
            Name = request.Name,
            Language = request.Language,
            Status = request.Status,
            SortType = request.SortType,
            TenantId = request.TenantId
        };

        await faqRepository.AddAsync(faq, cancellationToken);
        await faqRepository.SaveChangesAsync(cancellationToken);

        return faq.Id;
    }
}