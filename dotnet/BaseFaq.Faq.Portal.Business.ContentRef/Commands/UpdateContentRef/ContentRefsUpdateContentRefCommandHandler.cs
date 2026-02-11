using BaseFaq.Common.Infrastructure.ApiErrorHandling.Exception;
using BaseFaq.Faq.Common.Persistence.FaqDb;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BaseFaq.Faq.Portal.Business.ContentRef.Commands.UpdateContentRef;

public class ContentRefsUpdateContentRefCommandHandler(FaqDbContext dbContext)
    : IRequestHandler<ContentRefsUpdateContentRefCommand>
{
    public async Task Handle(ContentRefsUpdateContentRefCommand request, CancellationToken cancellationToken)
    {
        var contentRef = await dbContext.ContentRefs.FirstOrDefaultAsync(
            entity => entity.Id == request.Id,
            cancellationToken);
        if (contentRef is null)
        {
            throw new ApiErrorException(
                $"Content reference '{request.Id}' was not found.",
                errorCode: (int)HttpStatusCode.NotFound);
        }

        contentRef.Kind = request.Kind;
        contentRef.Locator = request.Locator;
        contentRef.Label = request.Label;
        contentRef.Scope = request.Scope;

        dbContext.ContentRefs.Update(contentRef);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}