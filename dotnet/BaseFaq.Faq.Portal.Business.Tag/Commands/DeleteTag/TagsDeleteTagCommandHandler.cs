using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.Portal.Business.Tag.Commands.DeleteTag;

public class TagsDeleteTagCommandHandler(FaqDbContext dbContext) : IRequestHandler<TagsDeleteTagCommand>
{
    public async Task Handle(TagsDeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await dbContext.Tags
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (tag is null)
        {
            throw new ApiErrorException(
                $"Tag '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        dbContext.Tags.Remove(tag);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}