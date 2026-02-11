using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.Portal.Business.Faq.Commands.UpdateFaqContentRef;

public class FaqContentRefsUpdateFaqContentRefCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqContentRefsUpdateFaqContentRefCommand>
{
    public async Task Handle(FaqContentRefsUpdateFaqContentRefCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var faqContentRef = await dbContext.FaqContentRefs.FirstOrDefaultAsync(
            entity => entity.Id == request.Id,
            cancellationToken);
        if (faqContentRef is null)
        {
            throw new ApiErrorException(
                $"FAQ content reference '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        var faqExists = await dbContext.Faqs.AnyAsync(entity => entity.Id == request.FaqId, cancellationToken);
        if (!faqExists)
        {
            throw new ApiErrorException(
                $"FAQ '{request.FaqId}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        var contentRefExists = await dbContext.ContentRefs.AnyAsync(
            entity => entity.Id == request.ContentRefId,
            cancellationToken);
        if (!contentRefExists)
        {
            throw new ApiErrorException(
                $"Content reference '{request.ContentRefId}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        faqContentRef.FaqId = request.FaqId;
        faqContentRef.ContentRefId = request.ContentRefId;

        dbContext.FaqContentRefs.Update(faqContentRef);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}