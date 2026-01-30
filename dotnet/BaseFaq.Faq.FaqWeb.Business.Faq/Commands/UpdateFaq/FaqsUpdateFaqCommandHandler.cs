using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaq;

public class FaqsUpdateFaqCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqsUpdateFaqCommand>
{
    public async Task Handle(FaqsUpdateFaqCommand request, CancellationToken cancellationToken)
    {
        var faq = await dbContext.Faqs.FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (faq is null)
        {
            throw new KeyNotFoundException($"FAQ '{request.Id}' was not found.");
        }

        faq.Name = request.Name;
        faq.Language = request.Language;
        faq.Status = request.Status;
        faq.SortType = request.SortType;

        dbContext.Faqs.Update(faq);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}