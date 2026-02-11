using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.FaqWeb.Business.ContentRef.Commands.DeleteContentRef;

public class ContentRefsDeleteContentRefCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<ContentRefsDeleteContentRefCommand>
{
    public async Task Handle(ContentRefsDeleteContentRefCommand request, CancellationToken cancellationToken)
    {
        var contentRef = await dbContext.ContentRefs
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);
        if (contentRef is null)
        {
            throw new ApiErrorException(
                $"Content ref '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        dbContext.ContentRefs.Remove(contentRef);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}