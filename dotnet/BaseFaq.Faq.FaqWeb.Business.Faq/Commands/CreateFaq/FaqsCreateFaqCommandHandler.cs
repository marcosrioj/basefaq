using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaq;

public class FaqsCreateFaqCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqsCreateFaqCommand, Guid>
{
    public async Task<Guid> Handle(FaqsCreateFaqCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tagIds = request.TagIds.Distinct().ToList();
        var contentRefIds = request.ContentRefIds.Distinct().ToList();

        var tags = tagIds.Count == 0
            ? []
            : await dbContext.Tags
                .Where(tag => tagIds.Contains(tag.Id))
                .ToListAsync(cancellationToken);

        var contentRefs = contentRefIds.Count == 0
            ? []
            : await dbContext.ContentRefs
                .Where(contentRef => contentRefIds.Contains(contentRef.Id))
                .ToListAsync(cancellationToken);

        if (tags.Count != tagIds.Count)
        {
            throw new ApiErrorException(
                "One or more tags were not found.",
                errorCode: (int)HttpStatusCode.BadRequest);
        }

        if (contentRefs.Count != contentRefIds.Count)
        {
            throw new ApiErrorException(
                "One or more content references were not found.",
                errorCode: (int)HttpStatusCode.BadRequest);
        }

        var faq = new FaqWeb.Persistence.FaqDb.Entities.Faq
        {
            Name = request.Name,
            Language = request.Language,
            Status = request.Status,
            SortStrategy = request.SortStrategy,
            CtaEnabled = request.CtaEnabled,
            CtaTarget = request.CtaTarget,
            TenantId = request.TenantId
        };

        foreach (var tag in tags)
        {
            faq.Tags.Add(new Persistence.FaqDb.Entities.FaqTag
            {
                TenantId = request.TenantId,
                Faq = faq,
                Tag = tag,
                TagId = tag.Id
            });
        }

        foreach (var contentRef in contentRefs)
        {
            faq.ContentRefs.Add(new Persistence.FaqDb.Entities.FaqContentRef
            {
                TenantId = request.TenantId,
                Faq = faq,
                ContentRef = contentRef,
                ContentRefId = contentRef.Id
            });
        }

        await dbContext.Faqs.AddAsync(faq, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return faq.Id;
    }
}