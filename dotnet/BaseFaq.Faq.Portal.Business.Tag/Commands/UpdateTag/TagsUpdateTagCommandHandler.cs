using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.Portal.Business.Tag.Commands.UpdateTag;

public class TagsUpdateTagCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<TagsUpdateTagCommand>
{
    public async Task Handle(TagsUpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await dbContext.Tags.FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (tag is null)
        {
            throw new ApiErrorException(
                $"Tag '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        tag.Value = request.Value;

        dbContext.Tags.Update(tag);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}