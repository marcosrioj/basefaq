using BaseFaq.Faq.Persistence.FaqDb.Repositories;
using MediatR;

namespace BaseFaq.Faq.Business.Faq.Commands.UpdateFaq;

public class FaqsUpdateFaqCommandHandler(IFaqRepository faqRepository) : IRequestHandler<FaqsUpdateFaqCommand>
{
    public async Task Handle(FaqsUpdateFaqCommand request, CancellationToken cancellationToken)
    {
        var faq = await faqRepository.GetByIdAsync(request.Id, cancellationToken);
        if (faq is null)
        {
            throw new KeyNotFoundException($"FAQ '{request.Id}' was not found.");
        }

        faq.Name = request.Name;
        faq.Language = request.Language;
        faq.Status = request.Status;
        faq.SortType = request.SortType;

        faqRepository.Update(faq);
        await faqRepository.SaveChangesAsync(cancellationToken);
    }
}