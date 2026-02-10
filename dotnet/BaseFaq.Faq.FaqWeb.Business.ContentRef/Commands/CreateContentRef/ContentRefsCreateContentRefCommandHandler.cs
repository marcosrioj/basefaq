using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using BaseFaq.Models.Common.Enums;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.ContentRef.Commands.CreateContentRef;

public class ContentRefsCreateContentRefCommandHandler(FaqDbContext dbContext, ISessionService sessionService)
    : IRequestHandler<ContentRefsCreateContentRefCommand, Guid>
{
    public async Task<Guid> Handle(ContentRefsCreateContentRefCommand request, CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(AppEnum.FaqWeb);

        var contentRef = new Persistence.FaqDb.Entities.ContentRef
        {
            Kind = request.Kind,
            Locator = request.Locator,
            Label = request.Label,
            Scope = request.Scope,
            TenantId = tenantId
        };

        await dbContext.ContentRefs.AddAsync(contentRef, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return contentRef.Id;
    }
}