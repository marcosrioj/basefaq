using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.DeleteFaqTag;

public class FaqTagsDeleteFaqTagCommandHandler(FaqDbContext dbContext) : IRequestHandler<FaqTagsDeleteFaqTagCommand>
{
    public async Task Handle(FaqTagsDeleteFaqTagCommand request, CancellationToken cancellationToken)
    {
        var faqTag = await dbContext.FaqTags
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (faqTag is null)
        {
            throw new ApiErrorException(
                $"FAQ tag '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        dbContext.FaqTags.Remove(faqTag);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}