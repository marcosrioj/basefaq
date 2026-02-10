using BaseFaq.Faq.FaqWeb.Persistence.FaqDb;
using BaseFaq.Faq.FaqWeb.Persistence.FaqDb.Entities;
using MediatR;

namespace BaseFaq.Faq.FaqWeb.Business.Tag.Commands.CreateTag;

public class TagsCreateTagCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<TagsCreateTagCommand, Guid>
{
    public async Task<Guid> Handle(TagsCreateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = new Persistence.FaqDb.Entities.Tag
        {
            Value = request.Value,
            TenantId = request.TenantId
        };

        await dbContext.Tags.AddAsync(tag, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return tag.Id;
    }
}