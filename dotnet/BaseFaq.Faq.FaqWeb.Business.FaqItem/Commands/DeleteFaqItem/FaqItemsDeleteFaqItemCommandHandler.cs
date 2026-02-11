using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.FaqItem.Commands.DeleteFaqItem;

public class FaqItemsDeleteFaqItemCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqItemsDeleteFaqItemCommand>
{
    public async Task Handle(FaqItemsDeleteFaqItemCommand request, CancellationToken cancellationToken)
    {
        var faqItem = await dbContext.FaqItems
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (faqItem is null)
        {
            throw new ApiErrorException(
                $"FAQ item '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        dbContext.FaqItems.Remove(faqItem);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}