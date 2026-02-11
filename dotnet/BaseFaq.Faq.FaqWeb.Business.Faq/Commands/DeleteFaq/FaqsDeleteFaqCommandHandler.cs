using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.DeleteFaq;

public class FaqsDeleteFaqCommandHandler(FaqDbContext dbContext) : IRequestHandler<FaqsDeleteFaqCommand>
{
    public async Task Handle(FaqsDeleteFaqCommand request, CancellationToken cancellationToken)
    {
        var faq = await dbContext.Faqs
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (faq is null)
        {
            throw new ApiErrorException(
                $"FAQ '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        dbContext.Faqs.Remove(faq);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}