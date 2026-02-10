using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.UpdateFaq;

public class FaqsUpdateFaqCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<FaqsUpdateFaqCommand>
{
    public async Task Handle(FaqsUpdateFaqCommand request, CancellationToken cancellationToken)
    {
        var faq = await dbContext.Faqs
            .Include(entity => entity.Tags)
            .Include(entity => entity.ContentRefs)
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (faq is null)
        {
            throw new ApiErrorException(
                $"FAQ '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        faq.Name = request.Name;
        faq.Language = request.Language;
        faq.Status = request.Status;
        faq.SortStrategy = request.SortStrategy;
        faq.CtaEnabled = request.CtaEnabled;
        faq.CtaTarget = request.CtaTarget;

        if (request.TagIds is not null)
        {
            var tagIds = request.TagIds.Distinct().ToList();
            var tags = tagIds.Count == 0
                ? []
                : await dbContext.Tags
                    .Where(tag => tagIds.Contains(tag.Id))
                    .ToListAsync(cancellationToken);

            if (tags.Count != tagIds.Count)
            {
                throw new ApiErrorException(
                    "One or more tags were not found.",
                    errorCode: (int)HttpStatusCode.BadRequest);
            }

            var existingTagIds = faq.Tags.Select(tag => tag.TagId).ToHashSet();
            var tagsToRemove = faq.Tags.Where(tag => !tagIds.Contains(tag.TagId)).ToList();

            foreach (var tag in tagsToRemove)
            {
                faq.Tags.Remove(tag);
            }

            foreach (var tag in tags.Where(tag => !existingTagIds.Contains(tag.Id)))
            {
                faq.Tags.Add(new Persistence.FaqDb.Entities.FaqTag
                {
                    TenantId = faq.TenantId,
                    Faq = faq,
                    Tag = tag,
                    TagId = tag.Id
                });
            }
        }

        if (request.ContentRefIds is not null)
        {
            var contentRefIds = request.ContentRefIds.Distinct().ToList();
            var contentRefs = contentRefIds.Count == 0
                ? []
                : await dbContext.ContentRefs
                    .Where(contentRef => contentRefIds.Contains(contentRef.Id))
                    .ToListAsync(cancellationToken);

            if (contentRefs.Count != contentRefIds.Count)
            {
                throw new ApiErrorException(
                    "One or more content references were not found.",
                    errorCode: (int)HttpStatusCode.BadRequest);
            }

            var existingContentRefIds = faq.ContentRefs.Select(contentRef => contentRef.ContentRefId).ToHashSet();
            var refsToRemove = faq.ContentRefs.Where(contentRef => !contentRefIds.Contains(contentRef.ContentRefId))
                .ToList();

            foreach (var contentRef in refsToRemove)
            {
                faq.ContentRefs.Remove(contentRef);
            }

            foreach (var contentRef in contentRefs.Where(contentRef => !existingContentRefIds.Contains(contentRef.Id)))
            {
                faq.ContentRefs.Add(new Persistence.FaqDb.Entities.FaqContentRef
                {
                    TenantId = faq.TenantId,
                    Faq = faq,
                    ContentRef = contentRef,
                    ContentRefId = contentRef.Id
                });
            }
        }

        dbContext.Faqs.Update(faq);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}