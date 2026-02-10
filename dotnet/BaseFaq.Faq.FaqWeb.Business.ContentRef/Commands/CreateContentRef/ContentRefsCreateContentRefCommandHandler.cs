using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.ContentRef.Commands.CreateContentRef;

public class ContentRefsCreateContentRefCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<ContentRefsCreateContentRefCommand, Guid>
{
    public async Task<Guid> Handle(ContentRefsCreateContentRefCommand request, CancellationToken cancellationToken)
    {
        var contentRef = new Persistence.FaqDb.Entities.ContentRef
        {
            Kind = request.Kind,
            Locator = request.Locator,
            Label = request.Label,
            Scope = request.Scope,
            TenantId = request.TenantId
        };

        await dbContext.ContentRefs.AddAsync(contentRef, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return contentRef.Id;
    }
}