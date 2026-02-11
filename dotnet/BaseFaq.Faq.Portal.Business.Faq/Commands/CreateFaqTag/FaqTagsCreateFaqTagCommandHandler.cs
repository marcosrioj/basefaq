using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Faq.Common.Persistence.FaqDb.Entities;
using BaseFaq.Models.Common.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.Portal.Business.Faq.Commands.CreateFaqTag;

public class FaqTagsCreateFaqTagCommandHandler(FaqDbContext dbContext, ISessionService sessionService)
    : IRequestHandler<FaqTagsCreateFaqTagCommand, Guid>
{
    public async Task<Guid> Handle(FaqTagsCreateFaqTagCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = sessionService.GetTenantId(AppEnum.Faq);

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

        var faqTag = new Common.Persistence.FaqDb.Entities.FaqTag
        {
            FaqId = request.FaqId,
            TagId = request.TagId,
            TenantId = tenantId
        };

        await dbContext.FaqTags.AddAsync(faqTag, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return faqTag.Id;
    }
}