using BaseFaq.Common.Infrastructure.Core.Abstractions;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using BaseFaq.Faq.Common.Persistence.FaqDb.Entities;
using BaseFaq.Models.Common.Enums;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Tag.Commands.CreateTag;

public class TagsCreateTagCommandHandler(FaqDbContext dbContext, ISessionService sessionService)
    : IRequestHandler<TagsCreateTagCommand, Guid>
{
    public async Task<Guid> Handle(TagsCreateTagCommand request, CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(AppEnum.Faq);

        var tag = new Common.Persistence.FaqDb.Entities.Tag
        {
            Value = request.Value,
            TenantId = tenantId
        };

        await dbContext.Tags.AddAsync(tag, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return tag.Id;
    }
}