using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaqTag;

public class FaqTagsUpdateFaqTagCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqTagsUpdateFaqTagCommand>
{
    public async Task Handle(FaqTagsUpdateFaqTagCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var faqTag = await dbContext.FaqTags.FirstOrDefaultAsync(
            entity => entity.Id == request.Id,
            cancellationToken);
        if (faqTag is null)
        {
            throw new ApiErrorException(
                $"FAQ tag '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        var faqExists = await dbContext.Faqs.AnyAsync(entity => entity.Id == request.FaqId, cancellationToken);
        if (!faqExists)
        {
            throw new ApiErrorException(
                $"FAQ '{request.FaqId}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        var tagExists = await dbContext.Tags.AnyAsync(entity => entity.Id == request.TagId, cancellationToken);
        if (!tagExists)
        {
            throw new ApiErrorException(
                $"Tag '{request.TagId}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        faqTag.FaqId = request.FaqId;
        faqTag.TagId = request.TagId;

        dbContext.FaqTags.Update(faqTag);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}