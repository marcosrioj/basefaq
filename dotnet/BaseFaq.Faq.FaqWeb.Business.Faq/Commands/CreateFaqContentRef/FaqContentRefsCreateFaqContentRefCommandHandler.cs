using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Models.Common.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.Faq.Commands.CreateFaqContentRef;

public class FaqContentRefsCreateFaqContentRefCommandHandler(
    FaqDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<FaqContentRefsCreateFaqContentRefCommand, Guid>
{
    public async Task<Guid> Handle(FaqContentRefsCreateFaqContentRefCommand request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tenantId = sessionService.GetTenantId(AppEnum.FaqWeb);

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

        var faqContentRef = new Persistence.FaqDb.Entities.FaqContentRef
        {
            FaqId = request.FaqId,
            ContentRefId = request.ContentRefId,
            TenantId = tenantId
        };

        await dbContext.FaqContentRefs.AddAsync(faqContentRef, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return faqContentRef.Id;
    }
}